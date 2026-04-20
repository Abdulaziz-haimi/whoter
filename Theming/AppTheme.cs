using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace water3.Theming
{
 

 
        internal static class AppTheme
        {
            // ===== Colors =====
            public static readonly Color FormBackColor = Color.FromArgb(243, 246, 250);
            public static readonly Color CardBackColor = Color.White;
            public static readonly Color CardBorderColor = Color.FromArgb(229, 231, 235);

            public static readonly Color TextPrimary = Color.FromArgb(17, 24, 39);
            public static readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
            public static readonly Color TextInput = Color.FromArgb(31, 41, 55);

            public static readonly Color GridHeaderBackColor = Color.FromArgb(248, 250, 252);
            public static readonly Color GridHeaderForeColor = Color.FromArgb(30, 41, 59);
            public static readonly Color GridLineColor = Color.FromArgb(235, 238, 242);
            public static readonly Color GridAltRowColor = Color.FromArgb(249, 250, 251);
            public static readonly Color GridSelectionBackColor = Color.FromArgb(219, 234, 254);

            public static readonly Color Success = Color.FromArgb(16, 185, 129);
            public static readonly Color Primary = Color.FromArgb(37, 99, 235);
            public static readonly Color Warning = Color.FromArgb(245, 158, 11);
            public static readonly Color Danger = Color.FromArgb(220, 38, 38);
            public static readonly Color Purple = Color.FromArgb(124, 58, 237);
            public static readonly Color Gray = Color.FromArgb(107, 114, 128);
            public static readonly Color DarkGray = Color.FromArgb(75, 85, 99);

            // ===== Fonts =====
            public static readonly Font DefaultFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            public static readonly Font DefaultBoldFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            public static readonly Font HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            public static readonly Font SubHeaderFont = new Font("Segoe UI", 8.75F, FontStyle.Regular);
            public static readonly Font GridHeaderFont = new Font("Segoe UI", 10F, FontStyle.Bold);

            // ===== Metrics =====
            public const int CardRadius = 18;
            public const int ButtonRadius = 12;
            public const int DefaultButtonSize = 42;
            public const int InputHeight = 38;
            public const int GridRowHeight = 40;
            public const int GridHeaderHeight = 42;
        }
    }