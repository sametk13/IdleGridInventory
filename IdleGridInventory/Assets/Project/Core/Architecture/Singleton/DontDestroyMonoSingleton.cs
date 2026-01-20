namespace OmniGameTemplate.Core.Architecture
{
    public class DontDestroyMonoSingleton : MonoSingleton<DontDestroyMonoSingleton>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}