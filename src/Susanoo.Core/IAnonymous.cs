using System.Dynamic;

namespace Susanoo
{
    /// <summary>
    /// Denotes that a type is a runtime generated type from Susanoo
    /// </summary>
    public interface IAnonymous
    {
        /// <summary>
        /// Allows quick retrieval and setting of fields on Susanoo's runtime generated types.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.Object.</returns>
        object this[string fieldName] { get; set; }
    }

    /// <summary>
    /// Runtime generated type from Susanoo
    /// </summary>
    public abstract class SusanooDynamic : DynamicObject, IAnonymous
    {
        public abstract object this[string fieldName] { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;

            return true;
        }
    }
}
