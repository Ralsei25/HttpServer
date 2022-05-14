using HttpServer.Helpers;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace HttpServer.Handlers
{
    public class ControllersHandler : IStreamHandler
    {
        private readonly Dictionary<string, Func<object>> _routes;
        private readonly ConcurrentDictionary<Type, Func<Task, object>> _extractors = new();

        public ControllersHandler(Assembly controllersAssemly)
        {
            _routes = controllersAssemly.GetTypes()
                .Where(x => typeof(IController).IsAssignableFrom(x))
                .SelectMany(Controller => Controller.GetMethods().Select(Method => new {
                    Controller,
                    Method
                }))
                .ToDictionary(
                    key => GetPath(key.Controller, key.Method),
                    value => GetEndpointMethod(value.Controller, value.Method)
                );
        }

        private Func<object> GetEndpointMethod(Type controller, MethodInfo method)
        {
            return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
        }

        private string GetPath(Type controller, MethodInfo method)
        {
            string name = controller.Name;
            if (name.EndsWith("controller", StringComparison.InvariantCultureIgnoreCase))
            {
                name = name.Substring(0, name.Length - "controller".Length);
            }
            if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"/{name}";
            }
            else
            {
                return $"/{name}/{method.Name}";
            }
        }

        public void Handle(Stream stream, Request request)
        {
            if (!_routes.TryGetValue(request.Path, out var func))
            {
                ResponseWriter.WriteStatus(HttpStatusCode.NotFound, stream);
            }
            else
            {
                ResponseWriter.WriteStatus(HttpStatusCode.OK, stream);
                WriteControllerResponse(func(), stream);
            }
        }
        public async Task HandleAsync(Stream stream, Request request)
        {
            if (!_routes.TryGetValue(request.Path, out var func))
            {
                await ResponseWriter.WriteStatusAsync(HttpStatusCode.NotFound, stream);
            }
            else
            {
                await ResponseWriter.WriteStatusAsync(HttpStatusCode.OK, stream);
                await WriteControllerResponseAsync(func(), stream);
            }
        }

        private void WriteControllerResponse(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write(str);
            }
            else if (response is byte[] buffer)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                WriteControllerResponse(JsonConvert.SerializeObject(response), stream);
            }
        }
        private async Task WriteControllerResponseAsync(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync(str);
            }
            else if (response is byte[] buffer)
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            else if (response is Task task)
            {
                await task;
                var result = ExtractValue(task);
                await WriteControllerResponseAsync(JsonConvert.SerializeObject(result), stream);
            }
            else
            {
                await WriteControllerResponseAsync(JsonConvert.SerializeObject(response), stream);
            }
        }
        private object? ExtractValue(Task task)
        {
            var taskType = task.GetType();
            if (!taskType.IsGenericType)
            {
                return null;
            }
            if (!_extractors.TryGetValue(taskType, out var extractor))
            {
                _extractors.TryAdd(taskType, extractor = CreateExtractor(taskType));
            }
            return extractor(task);
        }

        private Func<Task, object> CreateExtractor(Type taskType)
        {
            var param = Expression.Parameter(typeof(Task));
            var result = (Func<Task, object>)Expression.Lambda(
                typeof(Func<Task, object>), 
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(param, taskType), "Result"), 
                    typeof(object)), 
                param).Compile();
            return result;
        }

    }
}
