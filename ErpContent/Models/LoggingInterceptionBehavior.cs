using log4net;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Text;
using System.Collections.Generic;


namespace ErpContent
{

    public class LoggingInterceptionBehavior : IInterceptionBehavior
    {

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {

            var start = DateTime.Now;

            // Before invoking the method on the original target.
            if (log.IsDebugEnabled)
            {
                var sb = new StringBuilder();

                for (int i = 0; i < input.Inputs.Count; i++)
                {
                    var p = input.Inputs.GetParameterInfo(i);
                    var a = i <= input.Arguments.Count ? input.Arguments[i] : p.DefaultValue;

                    FieldInfo[] fields = a == null ? (new FieldInfo[] {}) :   a.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    if (fields.Length > 0)
                    {
                        sb.Append(string.Format("***********\nfields for {0}\n", p.Name));
                        for (int k = 0; k < fields.Length; k++)
                        {
                            sb.Append(String.Format(" {0} = {1}\n", fields[k].Name, fields[k].GetValue(a)));
                        }
                        sb.Append(string.Format("***********\n"));
                    }
                    else
                    {
                        sb.Append(string.Format("{0}={1}", p.Name, a));
                    }


                }
                if( sb.Length > 0 )
                   log.DebugFormat("\ncall {0} \nfor {1} \nwith inputs {2}\n", input.MethodBase, input.Target.GetType().FullName, sb.ToString());
                else
                    log.DebugFormat("\ncall {0} \nfor {1}\n", input.MethodBase, input.Target.GetType().FullName);
            }



            // Invoke the next behavior in the chain.
            var result = getNext()(input, getNext);

            // After invoking the method on the original target.
            if (result.Exception != null)
            {
                if (log.IsInfoEnabled)
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < input.Inputs.Count; i++)
                    {
                        var p = input.Inputs.GetParameterInfo(i);
                        var a = i <= input.Arguments.Count ? input.Arguments[i] : p.DefaultValue;

                        FieldInfo[] fields = a == null ? new FieldInfo[] {} :  a.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

                        if (i > 0)
                        {
                            sb.Append(", ");
                        }

                        if (fields.Length > 0)
                        {
                            sb.Append(string.Format("***********\nfields for {0}\n", p.Name));
                            for (int k = 0; k < fields.Length; k++)
                            {
                                sb.Append(String.Format("{0} = {1}\n", fields[k].Name, fields[k].GetValue(a)));
                            }
                            sb.Append(string.Format("***********\n"));
                        }
                        else
                        {
                            sb.Append(string.Format("{0}={1}", p.Name, a));
                        }

                    }

                    if (sb.Length > 0)
                        log.InfoFormat("\ncall {0} \nfor {1} \nwith inputs {2}\n", input.MethodBase, input.Target.GetType().FullName, sb.ToString());
                    else
                        log.InfoFormat("\ncall {0} \nfor {1}\n", input.MethodBase, input.Target.GetType().FullName);
                }

                Exception ex = result.Exception;


                if (ex.InnerException != null)
                {
                    log.ErrorFormat("{0}\nthrew inner exception {1}\n{2}",
                        input.MethodBase,
                        ex.InnerException.Message, ex.InnerException.StackTrace);
                }


                log.ErrorFormat("{0}\nthrew exception {1}\n{2}",
                    input.MethodBase,
                    ex.Message,ex.StackTrace);




            }
            else
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat("\n{0} for {1} \nreturned {2}\nduration {3}\n",
                      input.MethodBase, input.Target.GetType().FullName, result.ReturnValue, TimeSpan.FromTicks(DateTime.Now.Ticks - start.Ticks));
            }

            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }

    }
}
