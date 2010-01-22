using System;
using System.Linq;
using System.Reflection;

namespace QueryParser
{
    public static class PropertyNameResolver
    {
        /// <summary>
        /// Finds a particular property this occurs in <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">Type from which to retrieve property</typeparam>
        /// <param name="property">Name of the property (not case-sensitive)</param>
        /// <returns>
        /// PropertyInfo object of the property, null if not found
        /// </returns>
        /// <remarks>
        /// If a property is marked with <see cref="ParserIgnoreAttribute"/> null will be returned.
        /// 
        /// If a property is marked with <see cref="ParserPropertyName"/>, the name expected in this method
        /// will be the property name as specified by the property attribute. The other name of the property
        /// (normal property name in metadata) will be ignored for a property with this attribute.
        /// </remarks>
        public static PropertyInfo FindProperty<T>(string property)
        {
            var props = from prop in typeof(T).GetProperties()
                        where prop.GetCustomAttributes(typeof(ParserIgnoreAttribute), true).Length == 0
                        let propattr = prop.GetCustomAttributes(typeof(ParserPropertyName), true).SingleOrDefault()
                        let propertyName = propattr == null ? prop.Name : (propattr as ParserPropertyName).PropertyName
                        where propertyName.Equals(property, StringComparison.OrdinalIgnoreCase)
                        select prop;
            return props.SingleOrDefault();
        }
    }
}
