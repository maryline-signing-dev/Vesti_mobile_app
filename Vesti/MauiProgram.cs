using Microsoft.Extensions.Logging;

namespace Vesti;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // ── UI : Inter ────────────────────────────────────────────
                // Téléchargez les fichiers TTF depuis https://fonts.google.com/specimen/Inter
                // et placez-les dans Resources/Fonts/
                fonts.AddFont("Inter-Regular.ttf", "Inter");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");

                // ── Logo : DM Serif Display ────────────────────────────────
                // Fichier TTF : https://fonts.google.com/specimen/DM+Serif+Display
                fonts.AddFont("DMSerifDisplay-Regular.ttf", "DMSerifDisplay");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
