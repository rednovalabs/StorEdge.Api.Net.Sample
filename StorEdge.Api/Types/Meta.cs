using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorEdge.Api.Types {
    using Values = Dictionary<string, string>;

    public class Meta {
        public Pagination pagination;
        public int status_code;
        public String status_message;
        public String request_method;
        public Values configuration;
        public Values parameters;

        public int configuration_int(string key, int default_value = 0) {
            try {
                return int.Parse(configuration[key]);
            }
            catch {
                return default_value;
           }
        }

        public string configuration_string(string key, string default_value = "") {
            try {
                return configuration[key];
            }
            catch {
                return default_value;
            }
        }
    }
}
