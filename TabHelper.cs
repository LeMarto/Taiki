namespace Taiki
{
    internal class TabHelper
    {
        private int _tabCount = 0;
        private string _token = "";
        public string Get()
        {
            return _token;
        }
        public void Inc()
        {
            _tabCount++;
            Regen();
        }
        public void Dec()
        {
            if (_tabCount > 0)
            {
                _tabCount--;
                Regen();
            }
        }
        private void Regen()
        {
            _token = "";
            for(int i = _tabCount; i>1; i--)
            {
                _token += '\t';
            }
        }
    }
}