using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Movies.Client.Helpers
{
    public class JsonSerializerOptionsWrapper
    {
        public JsonSerializerOptions Options { get; }

        public JsonSerializerOptionsWrapper()
        {
            Options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }
    }
}
