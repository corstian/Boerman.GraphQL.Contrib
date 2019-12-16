using GraphQL.Types;
using System.Collections.Generic;

namespace Boerman.GraphQL.Contrib
{
    public static class Context
    {
        /// <summary>
        /// Gets a nested property from an input type omitting the need to put a specific field within an object.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="context">The context from which to retrieve the argument</param>
        /// <param name="argument">The argument from which to retrieve the property</param>
        /// <param name="property">The property to retrieve</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TArgument GetNestedArgument<TContext, TArgument>(
                        this ResolveFieldContext<TContext> context,
                        string argument,
                        string property,
                        TArgument defaultValue = default)
        {
            if (context.Arguments.TryGetValue(argument, out var o))
            {
                if (o is Dictionary<string, object>)
                {
                    if (((Dictionary<string, object>)o).TryGetValue(property, out var prop))
                    {
                        if (prop is TArgument)
                        {
                            return (TArgument)prop;
                        }
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a nested property from an input type omitting the need to put a specific field within an object.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="context">The context from which to retrieve the argument</param>
        /// <param name="argument">The argument from which to retrieve the property</param>
        /// <param name="property">The property to retrieve</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetNestedArgument<T>(
            this ResolveFieldContext<object> context,
            string argument,
            string property,
            T defaultValue = default)
        {
            return context.GetNestedArgument<object, T>(argument, property, defaultValue);
        }
    }
}
