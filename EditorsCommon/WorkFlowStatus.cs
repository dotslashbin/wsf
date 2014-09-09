using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{
    public class WorkFlowState
    {


        /// <summary>
        /// all new entities should be in that state
        /// </summary>
        public const int DRAFT = 0;
        public const int READY_FOR_REVIEW = 10;
        public const int READY_FOR_PROOF_READ = 50;
        public const int READY_APPROVED = 60;
        /// <summary>
        /// all entities visible in production should have that state
        /// </summary>
        public const int PUBLISHED = 100;

        /// <summary>
        /// retired entities (notes, articles) should be in that state
        /// </summary>
        public const int ARCHIVED = 200;


        public const int STATE_GROUP_IN_PROCESS = 1;
        public const int STATE_GROUP_PUBLISHED = 2;
        public const int STATE_GROUP_ARCHIVED = 3;
        public const int STATE_GROUP_ALL = 99;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool IsInState(int state, int group){

            switch (group)
            {
                case STATE_GROUP_IN_PROCESS :
                    return state < PUBLISHED;

                case STATE_GROUP_PUBLISHED:
                    return state == PUBLISHED;

                case STATE_GROUP_ARCHIVED:
                    return state == ARCHIVED;

                case STATE_GROUP_ALL:
                    return true;

            }

            return false;

        }



    }
}
