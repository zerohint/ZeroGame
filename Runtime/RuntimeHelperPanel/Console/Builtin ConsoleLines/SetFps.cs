using UnityEngine;

namespace ZeroGame.RHP
{
    public class SetFps : RuntimeConsoleCommand
    {
        public override string Description => "Set Application.targetFrameRate";

        public override string Execute(string[] parameters)
        {
            if (!HasParameter(parameters)) return "Parameter needed.";
                
            if (!int.TryParse(parameters[0], out int targetFrameRate))
                return "Please enter a number for frame number.";

            Application.targetFrameRate = targetFrameRate;
            return $"Target frame rate set to {Application.targetFrameRate}.";
        }
    }
}