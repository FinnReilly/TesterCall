using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class DateTimeWrapper : IDateTimeWrapper
    {
        public DateTime Now => DateTime.Now;
    }
}
