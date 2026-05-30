namespace ZeroGame.RHP
{
    public abstract class RuntimeConsoleCommand
    {
        public virtual string Command => GetType().Name;
        public abstract string Description { get; }

        public abstract string Execute(string[] parameters);



        /// <summary>
        /// Is these parameters are exists
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected bool HasParameter(string[] args) => 1 <= args.Length && args[0] != null && args[0] != "";
    }
}
