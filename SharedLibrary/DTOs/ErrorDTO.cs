using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTOs
{
    public class ErrorDTO
    {
        public List<String>? Errors { get; private set; }
        public bool IsShown { get; private set; }

        public ErrorDTO()
        {
            Errors = new List<String>();
        }

        public ErrorDTO(string? error, bool isShown)
        {
            Errors?.Add(error);
            IsShown = isShown;
        }

        public ErrorDTO(List<string>? errors, bool isShown)
        {
            Errors = Errors;
            IsShown = isShown;
        }
    }
}
