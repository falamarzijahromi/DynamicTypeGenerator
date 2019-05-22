using System;
using System.Collections.Generic;

namespace DynamicTypeGenerator.Abstracts
{
    public interface IInvokationEvaluator
    {
	    object Evaluate(InvokactionContext context);
    }

    public class InvokactionContext
    {
	    private readonly IDictionary<object, Type> fields;
	    private readonly IDictionary<object, Type> @params;

		internal InvokactionContext()
        {
			fields = new Dictionary<object, Type>();
			@params = new Dictionary<object, Type>();
		}

	    internal void AddParameter(object paramObject)
	    {
		    @params.Add(paramObject, paramObject.GetType());
	    }

	    internal void AddField(object fieldObject)
	    {
		    fields.Add(fieldObject, fieldObject.GetType());
		}

	    public IDictionary<object, Type> Fields => new Dictionary<object, Type>(fields);
	    public IDictionary<object, Type> Params => new Dictionary<object, Type>(@params);
	}
}