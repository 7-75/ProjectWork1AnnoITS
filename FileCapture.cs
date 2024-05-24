using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;

namespace Prove
{
    
    public static class FileCapture
    {

        public static async Task<Capture> Execute(string path)
        {
            string source = path;
            if (path.Contains(".png"))
            {
                source = "temp.png";
                await Cli.Wrap("convert")
                    .WithArguments($"\"{path}\" -channel A -threshold 254 \"{source}\"") //escape " in path
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteAsync();
            }

            StringBuilder sb = new();
            await Cli.Wrap("tesseract")
                .WithArguments($"-l eng+ita \"{source}\" stdout") //escape " in path
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb))
                .ExecuteAsync();
            Capture capture = new(path, sb.ToString());
            return capture;

        }
        
    }

}