using Microsoft.Practices.Prism.StoreApps;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Socialalert.ViewModels
{
    sealed class ViewModelContractResolver : DefaultContractResolver
    {
        private readonly string baseNamespace;

        public ViewModelContractResolver(string baseNamespace) {
            this.baseNamespace = baseNamespace;
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return base.GetSerializableMembers(objectType).Where(MustSerialize).ToList();
        }

        private bool MustSerialize(MemberInfo member)
        {
            var property = member as PropertyInfo;
            if (property == null)
            {
                return false;
            }
            if (!property.DeclaringType.Namespace.StartsWith(baseNamespace))
            {
                return false;
            }
            if (IsExcludedType(property.PropertyType.GetTypeInfo()))
            {
                return false;
            }
            return true;
        }

        private bool IsExcludedType(TypeInfo type)
        {
            return typeof(ICommand).GetTypeInfo().IsAssignableFrom(type);
        }
    }
}
