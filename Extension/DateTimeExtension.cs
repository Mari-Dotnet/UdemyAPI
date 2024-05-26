using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extension
{
    public static class DateTimeExtension
    {
        public static int CalCulateAge(this DateTime dob){
            var today=DateOnly.FromDateTime(DateTime.UtcNow);
            var age= today.Year - dob.Year;
            return age;
        }
    }
}