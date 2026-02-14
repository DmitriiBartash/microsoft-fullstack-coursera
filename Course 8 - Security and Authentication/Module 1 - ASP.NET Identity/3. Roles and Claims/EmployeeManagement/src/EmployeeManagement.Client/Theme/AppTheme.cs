using MudBlazor;

namespace EmployeeManagement.Client.Theme;

public static class AppTheme
{
    public static MudTheme Default => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#4F46E5",
            Secondary = "#0D9488",
            AppbarBackground = "#4F46E5",
            Background = "#F8FAFC",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#334155",
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#3B82F6",
            Divider = "#E2E8F0",
            TextPrimary = "#1E293B",
            TextSecondary = "#64748B"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#818CF8",
            Secondary = "#2DD4BF",
            AppbarBackground = "#1E1B4B",
            Background = "#0F172A",
            Surface = "#1E293B",
            DrawerBackground = "#1E293B",
            DrawerText = "#CBD5E1",
            Success = "#34D399",
            Warning = "#FBBF24",
            Error = "#F87171",
            Info = "#60A5FA",
            Divider = "#334155",
            TextPrimary = "#F1F5F9",
            TextSecondary = "#94A3B8"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "Roboto", "Helvetica", "Arial", "sans-serif"],
                LetterSpacing = "-0.011em"
            },
            H4 = new H4Typography
            {
                FontWeight = "700"
            },
            H5 = new H5Typography
            {
                FontWeight = "600"
            },
            H6 = new H6Typography
            {
                FontWeight = "600"
            }
        }
    };
}
