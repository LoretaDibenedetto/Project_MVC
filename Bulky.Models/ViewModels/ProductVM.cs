﻿
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace Bulky.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product  { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }   
    }
}
