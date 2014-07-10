using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEdge.Api.Responses;

namespace StorEdge.Api {
    public class FacilityReader : BaseReader {
        public String facility_id { get; set; }

        public FacilityReader(String facility_id, String key, String secret) : base(key, secret) {
            this.facility_id = facility_id;
        }

        public FacilityResponse read_info() {
            BaseReader reader = define_request(String.Format("{0}/info", facility_id), typeof(FacilityResponse));
            return (FacilityResponse)reader.read_next_page();
        }
    }
}
