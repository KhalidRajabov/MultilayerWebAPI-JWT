using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTOs
{
    public class ErrorDTO
    {
        public List<String>? Errors { get; private set; }=new List<string>() { };
        public bool IsShown { get; private set; }

      
        public ErrorDTO(string? error, bool isShown)
        {
            Errors?.Add(error);
            IsShown = isShown;
        }

        public ErrorDTO(List<string>? errors, bool isShown)
        {
            Errors = errors;
            IsShown = isShown;
        }
    }
}
