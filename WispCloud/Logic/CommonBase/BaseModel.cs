using System;
using System.Collections.Generic;
using DeusCloud.Exceptions;

namespace DeusCloud.Logic.CommonBase
{
    public abstract class BaseModel
    {
        public virtual void Validate()
        {
        }

        protected void ValidateField(BaseModel model, string fieldName)
        {
            try
            {
                model.Validate();
            }
            catch (Exception exception)
            {
                exception.PrependField(fieldName);
                throw;
            }
        }

        protected void ValidateCollectionField(IEnumerable<BaseModel> models, string fieldName)
        {
            try
            {
                models.Validate();
            }
            catch (Exception exception)
            {
                exception.PrependField(fieldName);
                throw;
            }
        }
    }

    public static class BaseModelExtentions
    {
        public static void Validate(this IEnumerable<BaseModel> models)
        {
            int index = 0;
            foreach (var model in models)
            {
                try
                {
                    model.Validate();
                    ++index;
                }
                catch (Exception exception)
                {
                    exception.PrependIndex(index);
                    throw;
                }
            }
        }

    }

}