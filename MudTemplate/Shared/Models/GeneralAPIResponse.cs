using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace MudTemplate.Shared.Models
{
    public class GeneralAPIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public object Result { get; set; } = default;

        public void AddModelStateErrors(ModelStateDictionary modelState)   
        {
            var errors = modelState.SelectMany(x => x.Value.Errors)
                 .Select(x => x.ErrorMessage).ToArray();

            ErrorMessages.AddRange(errors); 
        }
    }
}
