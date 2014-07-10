using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorEdge.Api.Types {
    public class Pagination {
        public Int32 current_page;
        public Int32 total_pages;
        public Int32 total_entries;
        public Int32 previous_page;
        public Int32 next_page;
    }
}
