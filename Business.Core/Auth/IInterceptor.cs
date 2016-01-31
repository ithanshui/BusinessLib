/*==================================
             ########
            ##########

             ########
            ##########
          ##############
         #######  #######
        ######      ######
        #####        #####
        ####          ####
        ####   ####   ####
        #####  ####  #####
         ################
          ##############
==================================*/

namespace Business.Auth
{
    public interface IInterception
    {
        System.Collections.Concurrent.ConcurrentDictionary<string, System.Func<object, object, Result.IResult>> Member { get; set; }

        System.Collections.Concurrent.ConcurrentDictionary<string, Business.Extensions.InterceptorCommand> Command { get; set; }

        Business.IBusiness Business { get; set; }
    }
}
