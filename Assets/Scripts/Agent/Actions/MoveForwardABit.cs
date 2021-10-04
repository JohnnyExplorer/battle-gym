
namespace Agent.Actions {
    public class MoveForwardABit
    {
        private int forward = 300;
        public int MoveForward()
        {
            if(forward>0){
                forward --;
                return -1 ;
            } else {
                return 0;
            }
        }
    }
}