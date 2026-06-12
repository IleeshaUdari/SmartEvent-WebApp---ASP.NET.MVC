using System.Collections.Generic;
using SmartEventWeb.Models;

namespace SmartEventWeb.ViewModels
{
    public class AdminInquiryIndexVM
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Replied { get; set; }

        public List<Inquiry> Items { get; set; } = new();
    }
}
