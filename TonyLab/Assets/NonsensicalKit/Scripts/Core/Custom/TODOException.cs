using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.Custom
{
    public class TODOException : Exception
    {
        private readonly string _message;
        public TODOException(string message)
        {
            _message = message;
        }
        public override string Message
        {
            get
            {
                return "TODO:" + _message;
            }
        }
    }
}


