//
//  ReflectionHelper.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Reflection;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;

namespace R7.News.Integrations
{
    public static class ReflectionHelper
    {
        public static MethodInfo TryGetMethod (Type type, string methodName, BindingFlags bindingFlags)
        {
            return type.GetMethods (bindingFlags)
                       .FirstOrDefault (m => m.Name == methodName);
        }

        public static MethodInfo TryGetMethod (Type type, string methodName, BindingFlags bindingFlags, int paramsCount)
        {
            return type.GetMethods (bindingFlags)
                       .FirstOrDefault (m => m.Name == methodName && m.GetParameters ().Count () == paramsCount);
        }

        public static ConstructorInfo TryGetCstor (Type type)
        {
            return type.GetConstructor (new Type [] { });
        }

        public static object New (Type type)
        {
            return TryGetCstor (type).Invoke (null);
        }

        public static Assembly TryLoadAssembly (string path)
        {
            try {
                return Assembly.LoadFrom (path);
            } catch (Exception ex) {
                var log = new LogInfo ();
                log.Exception = new ExceptionInfo (ex);
                log.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString ();
                EventLogController.Instance.AddLog (log);

                return null;
            }
        }
    }
}
