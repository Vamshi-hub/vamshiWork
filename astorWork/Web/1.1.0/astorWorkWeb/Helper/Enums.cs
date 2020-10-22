using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace astorWork
{
    public class Enums
    {
        public enum Operation
        {
            Save,
            Update,
            Delete
        }
        public enum ValveTypes
        {
            QA = 0
            , Production = 1
        }
        public enum ValveStatus
        {
            Open = 0
            , Close = 1
        }
    }

}