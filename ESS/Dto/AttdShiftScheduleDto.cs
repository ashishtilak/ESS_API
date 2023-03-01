using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ESS.Dto
{
    public class AttdShiftScheduleDto
    {
        public int YearMt { get; set; }
        public string EmpUnqId { get; set; }
        public string D01 { get; set; }
        public string D02 { get; set; }
        public string D03 { get; set; }
        public string D04 { get; set; }
        public string D05 { get; set; }
        public string D06 { get; set; }
        public string D07 { get; set; }
        public string D08 { get; set; }
        public string D09 { get; set; }
        public string D10 { get; set; }
        public string D11 { get; set; }
        public string D12 { get; set; }
        public string D13 { get; set; }
        public string D14 { get; set; }
        public string D15 { get; set; }
        public string D16 { get; set; }
        public string D17 { get; set; }
        public string D18 { get; set; }
        public string D19 { get; set; }
        public string D20 { get; set; }
        public string D21 { get; set; }
        public string D22 { get; set; }
        public string D23 { get; set; }
        public string D24 { get; set; }
        public string D25 { get; set; }
        public string D26 { get; set; }
        public string D27 { get; set; }
        public string D28 { get; set; }
        public string D29 { get; set; }
        public string D30 { get; set; }
        public string D31 { get; set; }

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(AttdShiftScheduleDto);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                if (myPropInfo != null)
                    return myPropInfo.GetValue(this, null);
                else
                    throw new Exception("Error in getting property");
            }
            set
            {
                Type myType = typeof(AttdShiftScheduleDto);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                if (myPropInfo != null)
                    myPropInfo.SetValue(this, value, null);
                else
                    throw new Exception("Error in setting property");
            }
        }
    }
}