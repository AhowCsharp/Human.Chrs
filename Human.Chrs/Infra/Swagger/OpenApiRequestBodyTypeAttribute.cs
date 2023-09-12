using System;

namespace Human.Chrs.Infra.Swagger
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OpenApiRequestBodyTypeAttribute : System.Attribute
    {
        public Type BodyType { get; }

        public string[] ContentTypes { get; }

        public OpenApiRequestBodyTypeAttribute(Type type, string[] contentTypes = null)
        {
            BodyType = type;

            if (contentTypes == null)
            {
                ContentTypes = new[] { "application/json" };
            }
            else
            {
                ContentTypes = contentTypes;
            }
        }
    }
}