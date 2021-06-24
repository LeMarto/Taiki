namespace Taiki
{
    internal class CommaHelper
    {
        private bool _first = true;
        private string _token;
        public CommaHelper(string token){
            this._token = token;
        }
        public CommaHelper(){
            this._token = ",";
        }
        public string Get()
        {
            if (_first)
            {
                _first = false;
                return "";
            }
            return this._token;
        }
        public void Reset()
        {
            _first = true;
        }
    }
}