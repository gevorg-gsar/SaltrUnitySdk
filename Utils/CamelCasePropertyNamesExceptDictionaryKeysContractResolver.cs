#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)


using System;
namespace Newtonsoft.Json.Serialization
{
    public class CamelCasePropertyNamesExceptDictionaryKeysContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);

            contract.PropertyNameResolver = propertyName => propertyName;

            return contract;
        }
    }
}
#endif