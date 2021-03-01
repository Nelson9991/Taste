using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Models.Helpers
{
    public class TrimmingModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var nombrePropiedad = bindingContext.ModelName;

            var proveedorDeValores = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            if (proveedorDeValores == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            // ReSharper disable once PatternAlwaysOfType
            if (proveedorDeValores.FirstValue is string str &&
                !string.IsNullOrEmpty(str))
            {
                bindingContext.Result = ModelBindingResult.Success(str.Trim());
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}