using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace viperOSK
{

    //
    // Summary:
    //     Key codes returned by Event.keyCode. These map directly to a physical key on
    //     the keyboard.
    public enum OSK_KeyCode
    {
        //
        // Summary:
        //     Not assigned (never returned as the result of a keystroke).
        None = 0,
        //
        // Summary:
        //     The backspace key.
        Backspace = 8,
        //
        // Summary:
        //     The tab key.
        Tab = 9,
        //
        // Summary:
        //     The Clear key.
        Clear = 12,
        //
        // Summary:
        //     Return key.
        Return = 13,
        //
        // Summary:
        //     Pause on PC machines.
        Pause = 19,
        //
        // Summary:
        //     Escape key.
        Escape = 27,
        //
        // Summary:
        //     Space key.
        Space = 32,
        //
        // Summary:
        //     Exclamation mark key '!'.
        Exclaim = 33,
        //
        // Summary:
        //     Double quote key '"'.
        DoubleQuote = 34,
        //
        // Summary:
        //     Hash key '#'.
        Hash = 35,
        //
        // Summary:
        //     Dollar sign key '$'.
        Dollar = 36,
        //
        // Summary:
        //     Percent '%' key.
        Percent = 37,
        //
        // Summary:
        //     Ampersand key '&'.
        Ampersand = 38,
        //
        // Summary:
        //     Quote key '.
        Quote = 39,
        //
        // Summary:
        //     Left Parenthesis key '('.
        LeftParen = 40,
        //
        // Summary:
        //     Right Parenthesis key ')'.
        RightParen = 41,
        //
        // Summary:
        //     Asterisk key '*'.
        Asterisk = 42,
        //
        // Summary:
        //     Plus key '+'.
        Plus = 43,
        //
        // Summary:
        //     Comma ',' key.
        Comma = 44,
        //
        // Summary:
        //     Minus '-' key.
        Minus = 45,
        //
        // Summary:
        //     Period '.' key.
        Period = 46,
        //
        // Summary:
        //     Slash '/' key.
        Slash = 47,
        //
        // Summary:
        //     The '0' key on the top of the alphanumeric keyboard.
        Alpha0 = 48,
        //
        // Summary:
        //     The '1' key on the top of the alphanumeric keyboard.
        Alpha1 = 49,
        //
        // Summary:
        //     The '2' key on the top of the alphanumeric keyboard.
        Alpha2 = 50,
        //
        // Summary:
        //     The '3' key on the top of the alphanumeric keyboard.
        Alpha3 = 51,
        //
        // Summary:
        //     The '4' key on the top of the alphanumeric keyboard.
        Alpha4 = 52,
        //
        // Summary:
        //     The '5' key on the top of the alphanumeric keyboard.
        Alpha5 = 53,
        //
        // Summary:
        //     The '6' key on the top of the alphanumeric keyboard.
        Alpha6 = 54,
        //
        // Summary:
        //     The '7' key on the top of the alphanumeric keyboard.
        Alpha7 = 55,
        //
        // Summary:
        //     The '8' key on the top of the alphanumeric keyboard.
        Alpha8 = 56,
        //
        // Summary:
        //     The '9' key on the top of the alphanumeric keyboard.
        Alpha9 = 57,
        //
        // Summary:
        //     Colon ':' key.
        Colon = 58,
        //
        // Summary:
        //     Semicolon ';' key.
        Semicolon = 59,
        //
        // Summary:
        //     Less than '<' key.
        Less = 60,
        //
        // Summary:
        //     Equals '=' key.
        Equals = 61,
        //
        // Summary:
        //     Greater than '>' key.
        Greater = 62,
        //
        // Summary:
        //     Question mark '?' key.
        Question = 63,
        //
        // Summary:
        //     At key '@'.
        At = 64,
        //
        // Summary:
        //     Left square bracket key '['.
        LeftBracket = 91,
        //
        // Summary:
        //     Backslash key '\'.
        Backslash = 92,
        //
        // Summary:
        //     Right square bracket key ']'.
        RightBracket = 93,
        //
        // Summary:
        //     Caret key '^'.
        Caret = 94,
        //
        // Summary:
        //     Underscore '_' key.
        Underscore = 95,
        //
        // Summary:
        //     Back quote key '`'.
        BackQuote = 96,
        //
        // Summary:
        //     'a' key.
        A = 97,
        //
        // Summary:
        //     'b' key.
        B = 98,
        //
        // Summary:
        //     'c' key.
        C = 99,
        //
        // Summary:
        //     'd' key.
        D = 100,
        //
        // Summary:
        //     'e' key.
        E = 101,
        //
        // Summary:
        //     'f' key.
        F = 102,
        //
        // Summary:
        //     'g' key.
        G = 103,
        //
        // Summary:
        //     'h' key.
        H = 104,
        //
        // Summary:
        //     'i' key.
        I = 105,
        //
        // Summary:
        //     'j' key.
        J = 106,
        //
        // Summary:
        //     'k' key.
        K = 107,
        //
        // Summary:
        //     'l' key.
        L = 108,
        //
        // Summary:
        //     'm' key.
        M = 109,
        //
        // Summary:
        //     'n' key.
        N = 110,
        //
        // Summary:
        //     'o' key.
        O = 111,
        //
        // Summary:
        //     'p' key.
        P = 112,
        //
        // Summary:
        //     'q' key.
        Q = 113,
        //
        // Summary:
        //     'r' key.
        R = 114,
        //
        // Summary:
        //     's' key.
        S = 115,
        //
        // Summary:
        //     't' key.
        T = 116,
        //
        // Summary:
        //     'u' key.
        U = 117,
        //
        // Summary:
        //     'v' key.
        V = 118,
        //
        // Summary:
        //     'w' key.
        W = 119,
        //
        // Summary:
        //     'x' key.
        X = 120,
        //
        // Summary:
        //     'y' key.
        Y = 121,
        //
        // Summary:
        //     'z' key.
        Z = 122,
        //
        // Summary:
        //     Left curly bracket key '{'.
        LeftCurlyBracket = 123,
        //
        // Summary:
        //     Pipe '|' key.
        Pipe = 124,
        //
        // Summary:
        //     Right curly bracket key '}'.
        RightCurlyBracket = 125,
        //
        // Summary:
        //     Tilde '~' key.
        Tilde = 126,
        //
        // Summary:
        //     The forward delete key.
        Delete = 127,
        //
        // Summary:
        //     Numeric keypad 0.
        Keypad0 = 256,
        //
        // Summary:
        //     Numeric keypad 1.
        Keypad1 = 257,
        //
        // Summary:
        //     Numeric keypad 2.
        Keypad2 = 258,
        //
        // Summary:
        //     Numeric keypad 3.
        Keypad3 = 259,
        //
        // Summary:
        //     Numeric keypad 4.
        Keypad4 = 260,
        //
        // Summary:
        //     Numeric keypad 5.
        Keypad5 = 261,
        //
        // Summary:
        //     Numeric keypad 6.
        Keypad6 = 262,
        //
        // Summary:
        //     Numeric keypad 7.
        Keypad7 = 263,
        //
        // Summary:
        //     Numeric keypad 8.
        Keypad8 = 264,
        //
        // Summary:
        //     Numeric keypad 9.
        Keypad9 = 265,
        //
        // Summary:
        //     Numeric keypad '.'.
        KeypadPeriod = 266,
        //
        // Summary:
        //     Numeric keypad '/'.
        KeypadDivide = 267,
        //
        // Summary:
        //     Numeric keypad '*'.
        KeypadMultiply = 268,
        //
        // Summary:
        //     Numeric keypad '-'.
        KeypadMinus = 269,
        //
        // Summary:
        //     Numeric keypad '+'.
        KeypadPlus = 270,
        //
        // Summary:
        //     Numeric keypad Enter.
        KeypadEnter = 271,
        //
        // Summary:
        //     Numeric keypad '='.
        KeypadEquals = 272,
        //
        // Summary:
        //     Up arrow key.
        UpArrow = 273,
        //
        // Summary:
        //     Down arrow key.
        DownArrow = 274,
        //
        // Summary:
        //     Right arrow key.
        RightArrow = 275,
        //
        // Summary:
        //     Left arrow key.
        LeftArrow = 276,
        //
        // Summary:
        //     Insert key key.
        Insert = 277,
        //
        // Summary:
        //     Home key.
        Home = 278,
        //
        // Summary:
        //     End key.
        End = 279,
        //
        // Summary:
        //     Page up.
        PageUp = 280,
        //
        // Summary:
        //     Page down.
        PageDown = 281,
        //
        // Summary:
        //     F1 function key.
        F1 = 282,
        //
        // Summary:
        //     F2 function key.
        F2 = 283,
        //
        // Summary:
        //     F3 function key.
        F3 = 284,
        //
        // Summary:
        //     F4 function key.
        F4 = 285,
        //
        // Summary:
        //     F5 function key.
        F5 = 286,
        //
        // Summary:
        //     F6 function key.
        F6 = 287,
        //
        // Summary:
        //     F7 function key.
        F7 = 288,
        //
        // Summary:
        //     F8 function key.
        F8 = 289,
        //
        // Summary:
        //     F9 function key.
        F9 = 290,
        //
        // Summary:
        //     F10 function key.
        F10 = 291,
        //
        // Summary:
        //     F11 function key.
        F11 = 292,
        //
        // Summary:
        //     F12 function key.
        F12 = 293,
        //
        // Summary:
        //     F13 function key.
        F13 = 294,
        //
        // Summary:
        //     F14 function key.
        F14 = 295,
        //
        // Summary:
        //     F15 function key.
        F15 = 296,
        //
        // Summary:
        //     Numlock key.
        Numlock = 300,
        //
        // Summary:
        //     Capslock key.
        CapsLock = 301,
        //
        // Summary:
        //     Scroll lock key.
        ScrollLock = 302,
        //
        // Summary:
        //     Right shift key.
        RightShift = 303,
        //
        // Summary:
        //     Left shift key.
        LeftShift = 304,
        //
        // Summary:
        //     Right Control key.
        RightControl = 305,
        //
        // Summary:
        //     Left Control key.
        LeftControl = 306,
        //
        // Summary:
        //     Right Alt key.
        RightAlt = 307,
        //
        // Summary:
        //     Left Alt key.
        LeftAlt = 308,
        //
        // Summary:
        //     Maps to right Windows key or right Command key if physical keys are enabled in
        //     Input Manager settings, otherwise maps to right Command key only.
        RightMeta = 309,
        //
        // Summary:
        //     Right Command key.
        RightCommand = 309,
        //
        // Summary:
        //     Right Command key.
        RightApple = 309,
        //
        // Summary:
        //     Maps to left Windows key or left Command key if physical keys are enabled in
        //     Input Manager settings, otherwise maps to left Command key only.
        LeftMeta = 310,
        //
        // Summary:
        //     Left Command key.
        LeftCommand = 310,
        //
        // Summary:
        //     Left Command key.
        LeftApple = 310,
        //
        // Summary:
        //     Left Windows key.
        LeftWindows = 311,
        //
        // Summary:
        //     Right Windows key.
        RightWindows = 312,
        //
        // Summary:
        //     Alt Gr key.
        AltGr = 313,
        //
        // Summary:
        //     Help key.
        Help = 315,
        //
        // Summary:
        //     Print key.
        Print = 316,
        //
        // Summary:
        //     Sys Req key.
        SysReq = 317,
        //
        // Summary:
        //     Break key.
        Break = 318,
        //
        // Summary:
        //     Menu key.
        Menu = 319,
        //
        // Summary:
        //     The Left (or primary) mouse button.
        Mouse0 = 323,
        //
        // Summary:
        //     Right mouse button (or secondary mouse button).
        Mouse1 = 324,
        //
        // Summary:
        //     Middle mouse button (or third button).
        Mouse2 = 325,
        //
        // Summary:
        //     Additional (fourth) mouse button.
        Mouse3 = 326,
        //
        // Summary:
        //     Additional (fifth) mouse button.
        Mouse4 = 327,
        //
        // Summary:
        //     Additional (or sixth) mouse button.
        Mouse5 = 328,
        //
        // Summary:
        //     Additional (or seventh) mouse button.
        Mouse6 = 329,
        //
        // Summary:
        //     Button 0 on any joystick.
        JoystickButton0 = 330,
        //
        // Summary:
        //     Button 1 on any joystick.
        JoystickButton1 = 331,
        //
        // Summary:
        //     Button 2 on any joystick.
        JoystickButton2 = 332,
        //
        // Summary:
        //     Button 3 on any joystick.
        JoystickButton3 = 333,
        //
        // Summary:
        //     Button 4 on any joystick.
        JoystickButton4 = 334,
        //
        // Summary:
        //     Button 5 on any joystick.
        JoystickButton5 = 335,
        //
        // Summary:
        //     Button 6 on any joystick.
        JoystickButton6 = 336,
        //
        // Summary:
        //     Button 7 on any joystick.
        JoystickButton7 = 337,
        //
        // Summary:
        //     Button 8 on any joystick.
        JoystickButton8 = 338,
        //
        // Summary:
        //     Button 9 on any joystick.
        JoystickButton9 = 339,
        //
        // Summary:
        //     Button 10 on any joystick.
        JoystickButton10 = 340,
        //
        // Summary:
        //     Button 11 on any joystick.
        JoystickButton11 = 341,
        //
        // Summary:
        //     Button 12 on any joystick.
        JoystickButton12 = 342,
        //
        // Summary:
        //     Button 13 on any joystick.
        JoystickButton13 = 343,
        //
        // Summary:
        //     Button 14 on any joystick.
        JoystickButton14 = 344,
        //
        // Summary:
        //     Button 15 on any joystick.
        JoystickButton15 = 345,
        //
        // Summary:
        //     Button 16 on any joystick.
        JoystickButton16 = 346,
        //
        // Summary:
        //     Button 17 on any joystick.
        JoystickButton17 = 347,
        //
        // Summary:
        //     Button 18 on any joystick.
        JoystickButton18 = 348,
        //
        // Summary:
        //     Button 19 on any joystick.
        JoystickButton19 = 349,
        //
        // Summary:
        //     Button 0 on first joystick.
        Joystick1Button0 = 350,
        //
        // Summary:
        //     Button 1 on first joystick.
        Joystick1Button1 = 351,
        //
        // Summary:
        //     Button 2 on first joystick.
        Joystick1Button2 = 352,
        //
        // Summary:
        //     Button 3 on first joystick.
        Joystick1Button3 = 353,
        //
        // Summary:
        //     Button 4 on first joystick.
        Joystick1Button4 = 354,
        //
        // Summary:
        //     Button 5 on first joystick.
        Joystick1Button5 = 355,
        //
        // Summary:
        //     Button 6 on first joystick.
        Joystick1Button6 = 356,
        //
        // Summary:
        //     Button 7 on first joystick.
        Joystick1Button7 = 357,
        //
        // Summary:
        //     Button 8 on first joystick.
        Joystick1Button8 = 358,
        //
        // Summary:
        //     Button 9 on first joystick.
        Joystick1Button9 = 359,
        //
        // Summary:
        //     Button 10 on first joystick.
        Joystick1Button10 = 360,
        //
        // Summary:
        //     Button 11 on first joystick.
        Joystick1Button11 = 361,
        //
        // Summary:
        //     Button 12 on first joystick.
        Joystick1Button12 = 362,
        //
        // Summary:
        //     Button 13 on first joystick.
        Joystick1Button13 = 363,
        //
        // Summary:
        //     Button 14 on first joystick.
        Joystick1Button14 = 364,
        //
        // Summary:
        //     Button 15 on first joystick.
        Joystick1Button15 = 365,
        //
        // Summary:
        //     Button 16 on first joystick.
        Joystick1Button16 = 366,
        //
        // Summary:
        //     Button 17 on first joystick.
        Joystick1Button17 = 367,
        //
        // Summary:
        //     Button 18 on first joystick.
        Joystick1Button18 = 368,
        //
        // Summary:
        //     Button 19 on first joystick.
        Joystick1Button19 = 369,
        //
        // Summary:
        //     Button 0 on second joystick.
        Joystick2Button0 = 370,
        //
        // Summary:
        //     Button 1 on second joystick.
        Joystick2Button1 = 371,
        //
        // Summary:
        //     Button 2 on second joystick.
        Joystick2Button2 = 372,
        //
        // Summary:
        //     Button 3 on second joystick.
        Joystick2Button3 = 373,
        //
        // Summary:
        //     Button 4 on second joystick.
        Joystick2Button4 = 374,
        //
        // Summary:
        //     Button 5 on second joystick.
        Joystick2Button5 = 375,
        //
        // Summary:
        //     Button 6 on second joystick.
        Joystick2Button6 = 376,
        //
        // Summary:
        //     Button 7 on second joystick.
        Joystick2Button7 = 377,
        //
        // Summary:
        //     Button 8 on second joystick.
        Joystick2Button8 = 378,
        //
        // Summary:
        //     Button 9 on second joystick.
        Joystick2Button9 = 379,
        //
        // Summary:
        //     Button 10 on second joystick.
        Joystick2Button10 = 380,
        //
        // Summary:
        //     Button 11 on second joystick.
        Joystick2Button11 = 381,
        //
        // Summary:
        //     Button 12 on second joystick.
        Joystick2Button12 = 382,
        //
        // Summary:
        //     Button 13 on second joystick.
        Joystick2Button13 = 383,
        //
        // Summary:
        //     Button 14 on second joystick.
        Joystick2Button14 = 384,
        //
        // Summary:
        //     Button 15 on second joystick.
        Joystick2Button15 = 385,
        //
        // Summary:
        //     Button 16 on second joystick.
        Joystick2Button16 = 386,
        //
        // Summary:
        //     Button 17 on second joystick.
        Joystick2Button17 = 387,
        //
        // Summary:
        //     Button 18 on second joystick.
        Joystick2Button18 = 388,
        //
        // Summary:
        //     Button 19 on second joystick.
        Joystick2Button19 = 389,
        //
        // Summary:
        //     Button 0 on third joystick.
        Joystick3Button0 = 390,
        //
        // Summary:
        //     Button 1 on third joystick.
        Joystick3Button1 = 391,
        //
        // Summary:
        //     Button 2 on third joystick.
        Joystick3Button2 = 392,
        //
        // Summary:
        //     Button 3 on third joystick.
        Joystick3Button3 = 393,
        //
        // Summary:
        //     Button 4 on third joystick.
        Joystick3Button4 = 394,
        //
        // Summary:
        //     Button 5 on third joystick.
        Joystick3Button5 = 395,
        //
        // Summary:
        //     Button 6 on third joystick.
        Joystick3Button6 = 396,
        //
        // Summary:
        //     Button 7 on third joystick.
        Joystick3Button7 = 397,
        //
        // Summary:
        //     Button 8 on third joystick.
        Joystick3Button8 = 398,
        //
        // Summary:
        //     Button 9 on third joystick.
        Joystick3Button9 = 399,
        //
        // Summary:
        //     Button 10 on third joystick.
        Joystick3Button10 = 400,
        //
        // Summary:
        //     Button 11 on third joystick.
        Joystick3Button11 = 401,
        //
        // Summary:
        //     Button 12 on third joystick.
        Joystick3Button12 = 402,
        //
        // Summary:
        //     Button 13 on third joystick.
        Joystick3Button13 = 403,
        //
        // Summary:
        //     Button 14 on third joystick.
        Joystick3Button14 = 404,
        //
        // Summary:
        //     Button 15 on third joystick.
        Joystick3Button15 = 405,
        //
        // Summary:
        //     Button 16 on third joystick.
        Joystick3Button16 = 406,
        //
        // Summary:
        //     Button 17 on third joystick.
        Joystick3Button17 = 407,
        //
        // Summary:
        //     Button 18 on third joystick.
        Joystick3Button18 = 408,
        //
        // Summary:
        //     Button 19 on third joystick.
        Joystick3Button19 = 409,
        //
        // Summary:
        //     Button 0 on forth joystick.
        Joystick4Button0 = 410,
        //
        // Summary:
        //     Button 1 on forth joystick.
        Joystick4Button1 = 411,
        //
        // Summary:
        //     Button 2 on forth joystick.
        Joystick4Button2 = 412,
        //
        // Summary:
        //     Button 3 on forth joystick.
        Joystick4Button3 = 413,
        //
        // Summary:
        //     Button 4 on forth joystick.
        Joystick4Button4 = 414,
        //
        // Summary:
        //     Button 5 on forth joystick.
        Joystick4Button5 = 415,
        //
        // Summary:
        //     Button 6 on forth joystick.
        Joystick4Button6 = 416,
        //
        // Summary:
        //     Button 7 on forth joystick.
        Joystick4Button7 = 417,
        //
        // Summary:
        //     Button 8 on forth joystick.
        Joystick4Button8 = 418,
        //
        // Summary:
        //     Button 9 on forth joystick.
        Joystick4Button9 = 419,
        //
        // Summary:
        //     Button 10 on forth joystick.
        Joystick4Button10 = 420,
        //
        // Summary:
        //     Button 11 on forth joystick.
        Joystick4Button11 = 421,
        //
        // Summary:
        //     Button 12 on forth joystick.
        Joystick4Button12 = 422,
        //
        // Summary:
        //     Button 13 on forth joystick.
        Joystick4Button13 = 423,
        //
        // Summary:
        //     Button 14 on forth joystick.
        Joystick4Button14 = 424,
        //
        // Summary:
        //     Button 15 on forth joystick.
        Joystick4Button15 = 425,
        //
        // Summary:
        //     Button 16 on forth joystick.
        Joystick4Button16 = 426,
        //
        // Summary:
        //     Button 17 on forth joystick.
        Joystick4Button17 = 427,
        //
        // Summary:
        //     Button 18 on forth joystick.
        Joystick4Button18 = 428,
        //
        // Summary:
        //     Button 19 on forth joystick.
        Joystick4Button19 = 429,
        //
        // Summary:
        //     Button 0 on fifth joystick.
        Joystick5Button0 = 430,
        //
        // Summary:
        //     Button 1 on fifth joystick.
        Joystick5Button1 = 431,
        //
        // Summary:
        //     Button 2 on fifth joystick.
        Joystick5Button2 = 432,
        //
        // Summary:
        //     Button 3 on fifth joystick.
        Joystick5Button3 = 433,
        //
        // Summary:
        //     Button 4 on fifth joystick.
        Joystick5Button4 = 434,
        //
        // Summary:
        //     Button 5 on fifth joystick.
        Joystick5Button5 = 435,
        //
        // Summary:
        //     Button 6 on fifth joystick.
        Joystick5Button6 = 436,
        //
        // Summary:
        //     Button 7 on fifth joystick.
        Joystick5Button7 = 437,
        //
        // Summary:
        //     Button 8 on fifth joystick.
        Joystick5Button8 = 438,
        //
        // Summary:
        //     Button 9 on fifth joystick.
        Joystick5Button9 = 439,
        //
        // Summary:
        //     Button 10 on fifth joystick.
        Joystick5Button10 = 440,
        //
        // Summary:
        //     Button 11 on fifth joystick.
        Joystick5Button11 = 441,
        //
        // Summary:
        //     Button 12 on fifth joystick.
        Joystick5Button12 = 442,
        //
        // Summary:
        //     Button 13 on fifth joystick.
        Joystick5Button13 = 443,
        //
        // Summary:
        //     Button 14 on fifth joystick.
        Joystick5Button14 = 444,
        //
        // Summary:
        //     Button 15 on fifth joystick.
        Joystick5Button15 = 445,
        //
        // Summary:
        //     Button 16 on fifth joystick.
        Joystick5Button16 = 446,
        //
        // Summary:
        //     Button 17 on fifth joystick.
        Joystick5Button17 = 447,
        //
        // Summary:
        //     Button 18 on fifth joystick.
        Joystick5Button18 = 448,
        //
        // Summary:
        //     Button 19 on fifth joystick.
        Joystick5Button19 = 449,
        //
        // Summary:
        //     Button 0 on sixth joystick.
        Joystick6Button0 = 450,
        //
        // Summary:
        //     Button 1 on sixth joystick.
        Joystick6Button1 = 451,
        //
        // Summary:
        //     Button 2 on sixth joystick.
        Joystick6Button2 = 452,
        //
        // Summary:
        //     Button 3 on sixth joystick.
        Joystick6Button3 = 453,
        //
        // Summary:
        //     Button 4 on sixth joystick.
        Joystick6Button4 = 454,
        //
        // Summary:
        //     Button 5 on sixth joystick.
        Joystick6Button5 = 455,
        //
        // Summary:
        //     Button 6 on sixth joystick.
        Joystick6Button6 = 456,
        //
        // Summary:
        //     Button 7 on sixth joystick.
        Joystick6Button7 = 457,
        //
        // Summary:
        //     Button 8 on sixth joystick.
        Joystick6Button8 = 458,
        //
        // Summary:
        //     Button 9 on sixth joystick.
        Joystick6Button9 = 459,
        //
        // Summary:
        //     Button 10 on sixth joystick.
        Joystick6Button10 = 460,
        //
        // Summary:
        //     Button 11 on sixth joystick.
        Joystick6Button11 = 461,
        //
        // Summary:
        //     Button 12 on sixth joystick.
        Joystick6Button12 = 462,
        //
        // Summary:
        //     Button 13 on sixth joystick.
        Joystick6Button13 = 463,
        //
        // Summary:
        //     Button 14 on sixth joystick.
        Joystick6Button14 = 464,
        //
        // Summary:
        //     Button 15 on sixth joystick.
        Joystick6Button15 = 465,
        //
        // Summary:
        //     Button 16 on sixth joystick.
        Joystick6Button16 = 466,
        //
        // Summary:
        //     Button 17 on sixth joystick.
        Joystick6Button17 = 467,
        //
        // Summary:
        //     Button 18 on sixth joystick.
        Joystick6Button18 = 468,
        //
        // Summary:
        //     Button 19 on sixth joystick.
        Joystick6Button19 = 469,
        //
        // Summary:
        //     Button 0 on seventh joystick.
        Joystick7Button0 = 470,
        //
        // Summary:
        //     Button 1 on seventh joystick.
        Joystick7Button1 = 471,
        //
        // Summary:
        //     Button 2 on seventh joystick.
        Joystick7Button2 = 472,
        //
        // Summary:
        //     Button 3 on seventh joystick.
        Joystick7Button3 = 473,
        //
        // Summary:
        //     Button 4 on seventh joystick.
        Joystick7Button4 = 474,
        //
        // Summary:
        //     Button 5 on seventh joystick.
        Joystick7Button5 = 475,
        //
        // Summary:
        //     Button 6 on seventh joystick.
        Joystick7Button6 = 476,
        //
        // Summary:
        //     Button 7 on seventh joystick.
        Joystick7Button7 = 477,
        //
        // Summary:
        //     Button 8 on seventh joystick.
        Joystick7Button8 = 478,
        //
        // Summary:
        //     Button 9 on seventh joystick.
        Joystick7Button9 = 479,
        //
        // Summary:
        //     Button 10 on seventh joystick.
        Joystick7Button10 = 480,
        //
        // Summary:
        //     Button 11 on seventh joystick.
        Joystick7Button11 = 481,
        //
        // Summary:
        //     Button 12 on seventh joystick.
        Joystick7Button12 = 482,
        //
        // Summary:
        //     Button 13 on seventh joystick.
        Joystick7Button13 = 483,
        //
        // Summary:
        //     Button 14 on seventh joystick.
        Joystick7Button14 = 484,
        //
        // Summary:
        //     Button 15 on seventh joystick.
        Joystick7Button15 = 485,
        //
        // Summary:
        //     Button 16 on seventh joystick.
        Joystick7Button16 = 486,
        //
        // Summary:
        //     Button 17 on seventh joystick.
        Joystick7Button17 = 487,
        //
        // Summary:
        //     Button 18 on seventh joystick.
        Joystick7Button18 = 488,
        //
        // Summary:
        //     Button 19 on seventh joystick.
        Joystick7Button19 = 489,
        //
        // Summary:
        //     Button 0 on eighth joystick.
        Joystick8Button0 = 490,
        //
        // Summary:
        //     Button 1 on eighth joystick.
        Joystick8Button1 = 491,
        //
        // Summary:
        //     Button 2 on eighth joystick.
        Joystick8Button2 = 492,
        //
        // Summary:
        //     Button 3 on eighth joystick.
        Joystick8Button3 = 493,
        //
        // Summary:
        //     Button 4 on eighth joystick.
        Joystick8Button4 = 494,
        //
        // Summary:
        //     Button 5 on eighth joystick.
        Joystick8Button5 = 495,
        //
        // Summary:
        //     Button 6 on eighth joystick.
        Joystick8Button6 = 496,
        //
        // Summary:
        //     Button 7 on eighth joystick.
        Joystick8Button7 = 497,
        //
        // Summary:
        //     Button 8 on eighth joystick.
        Joystick8Button8 = 498,
        //
        // Summary:
        //     Button 9 on eighth joystick.
        Joystick8Button9 = 499,
        //
        // Summary:
        //     Button 10 on eighth joystick.
        Joystick8Button10 = 500,
        //
        // Summary:
        //     Button 11 on eighth joystick.
        Joystick8Button11 = 501,
        //
        // Summary:
        //     Button 12 on eighth joystick.
        Joystick8Button12 = 502,
        //
        // Summary:
        //     Button 13 on eighth joystick.
        Joystick8Button13 = 503,
        //
        // Summary:
        //     Button 14 on eighth joystick.
        Joystick8Button14 = 504,
        //
        // Summary:
        //     Button 15 on eighth joystick.
        Joystick8Button15 = 505,
        //
        // Summary:
        //     Button 16 on eighth joystick.
        Joystick8Button16 = 506,
        //
        // Summary:
        //     Button 17 on eighth joystick.
        Joystick8Button17 = 507,
        //
        // Summary:
        //     Button 18 on eighth joystick.
        Joystick8Button18 = 508,
        //
        // Summary:
        //     Button 19 on eighth joystick.
        Joystick8Button19 = 509,

        // viperOSK mapping for particular symbols
        // feel free to add more currencies or symbols as needed but use enums from 5001 to 5998 (see further below)
        // not all added symbols may not be represented in your font (so you'd have to make changes to that effect in chartoKeycode)
        // make sure not to replace any of the existing custom keys or change the enum
        // 
        __CUSTOM__ = 699,
        __SYMBOLS__ = 700,
        Cent = 701,
        Euro = 702,
        Sterling = 703,
        Yen = 704,
        Peso = 705,

        GreaterOrEqual = 763,

        SmallerOrEqual = 764,

        NotEqual = 765,


        _END_SYMBOLS__ = 800,

        _MINIKEYBOARD_ = 900,


        // viperOSK accent mapping
        // each character can have up to 10 accented variations
        // character derivations (accented..etc) occupy indecies 1000 - 4000 of this enum

        __ACCENTS__ = 1000,
        // Accent Codes for 'A'
        A_01 = 1101,
        A_02 = 1102,
        A_03 = 1103,
        A_04 = 1104,
        A_05 = 1105,
        A_06 = 1106,
        A_07 = 1107,
        A_08 = 1108,
        A_09 = 1109,
        A_10 = 1110,
        A_11 = 1111,
        A_12 = 1112,
        A_13 = 1113,
        A_14 = 1114,
        A_15 = 1115,
        A_16 = 1116,
        A_17 = 1117,
        A_18 = 1118,



        // Accent Codes for 'B'
        B_01 = 1201,
        B_02 = 1202,
        B_03 = 1203,
        B_04 = 1204,
        B_05 = 1205,
        B_06 = 1206,
        B_07 = 1207,
        B_08 = 1208,
        B_09 = 1209,
        B_10 = 1210,
        B_11 = 1211,
        B_12 = 1212,
        B_13 = 1213,
        B_14 = 1214,
        B_15 = 1215,
        B_16 = 1216,
        B_17 = 1217,
        B_18 = 1218,


        // Accent Codes for 'C'
        C_01 = 1301,
        C_02 = 1302,
        C_03 = 1303,
        C_04 = 1304,
        C_05 = 1305,
        C_06 = 1306,
        C_07 = 1307,
        C_08 = 1308,
        C_09 = 1309,
        C_10 = 1310,
        C_11 = 1311,
        C_12 = 1312,
        C_13 = 1313,
        C_14 = 1314,
        C_15 = 1315,
        C_16 = 1316,
        C_17 = 1317,
        C_18 = 1318,


        // Accent Codes for 'D'
        D_01 = 1301,
        D_02 = 1302,
        D_03 = 1303,
        D_04 = 1304,
        D_05 = 1305,
        D_06 = 1306,
        D_07 = 1307,
        D_08 = 1308,
        D_09 = 1309,
        D_10 = 1310,
        D_11 = 1311,
        D_12 = 1312,
        D_13 = 1313,
        D_14 = 1314,
        D_15 = 1315,
        D_16 = 1316,
        D_17 = 1317,
        D_18 = 1318,


        // Accent Codes for 'E'
        E_01 = 1401,
        E_02 = 1402,
        E_03 = 1403,
        E_04 = 1404,
        E_05 = 1405,
        E_06 = 1406,
        E_07 = 1407,
        E_08 = 1408,
        E_09 = 1409,
        E_10 = 1410,
        E_11 = 1411,
        E_12 = 1412,
        E_13 = 1413,
        E_14 = 1414,
        E_15 = 1415,
        E_16 = 1416,
        E_17 = 1417,
        E_18 = 1418,



        F_01 = 1601,
        F_02 = 1602,
        F_03 = 1603,
        F_04 = 1604,
        F_05 = 1605,
        F_06 = 1606,
        F_07 = 1607,
        F_08 = 1608,
        F_09 = 1609,
        F_10 = 1610,
        F_11 = 1611,
        F_12 = 1612,
        F_13 = 1613,
        F_14 = 1614,
        F_15 = 1615,
        F_16 = 1616,
        F_17 = 1617,
        F_18 = 1618,


        G_01 = 1701,
        G_02 = 1702,
        G_03 = 1703,
        G_04 = 1704,
        G_05 = 1705,
        G_06 = 1706,
        G_07 = 1707,
        G_08 = 1708,
        G_09 = 1709,
        G_10 = 1710,
        G_11 = 1711,
        G_12 = 1712,
        G_13 = 1713,
        G_14 = 1714,
        G_15 = 1715,
        G_16 = 1716,
        G_17 = 1717,
        G_18 = 1718,



        H_01 = 1801,
        H_02 = 1802,
        H_03 = 1803,
        H_04 = 1804,
        H_05 = 1805,
        H_06 = 1806,
        H_07 = 1807,
        H_08 = 1808,
        H_09 = 1809,
        H_10 = 1810,
        H_11 = 1811,
        H_12 = 1812,
        H_13 = 1813,
        H_14 = 1814,
        H_15 = 1815,
        H_16 = 1816,
        H_17 = 1817,
        H_18 = 1818,


        I_01 = 1901,
        I_02 = 1902,
        I_03 = 1903,
        I_04 = 1904,
        I_05 = 1905,
        I_06 = 1906,
        I_07 = 1907,
        I_08 = 1908,
        I_09 = 1909,
        I_10 = 1910,
        I_11 = 1911,
        I_12 = 1912,
        I_13 = 1913,
        I_14 = 1914,
        I_15 = 1915,
        I_16 = 1916,
        I_17 = 1917,
        I_18 = 1918,


        J_01 = 2001,
        J_02 = 2002,
        J_03 = 2003,
        J_04 = 2004,
        J_05 = 2005,
        J_06 = 2006,
        J_07 = 2007,
        J_08 = 2008,
        J_09 = 2009,
        J_10 = 2010,
        J_11 = 2011,
        J_12 = 2012,
        J_13 = 2013,
        J_14 = 2014,
        J_15 = 2015,
        J_16 = 2016,
        J_17 = 2017,
        J_18 = 2018,


        K_01 = 2101,
        K_02 = 2102,
        K_03 = 2103,
        K_04 = 2104,
        K_05 = 2105,
        K_06 = 2106,
        K_07 = 2107,
        K_08 = 2108,
        K_09 = 2109,
        K_10 = 2110,
        K_11 = 2111,
        K_12 = 2112,
        K_13 = 2113,
        K_14 = 2114,
        K_15 = 2115,
        K_16 = 2116,
        K_17 = 2117,
        K_18 = 2118,


        L_01 = 2201,
        L_02 = 2202,
        L_03 = 2203,
        L_04 = 2204,
        L_05 = 2205,
        L_06 = 2206,
        L_07 = 2207,
        L_08 = 2208,
        L_09 = 2209,
        L_10 = 2210,
        L_11 = 2211,
        L_12 = 2212,
        L_13 = 2213,
        L_14 = 2214,
        L_15 = 2215,
        L_16 = 2216,
        L_17 = 2217,
        L_18 = 2218,


        M_01 = 2301,
        M_02 = 2302,
        M_03 = 2303,
        M_04 = 2304,
        M_05 = 2305,
        M_06 = 2306,
        M_07 = 2307,
        M_08 = 2308,
        M_09 = 2309,
        M_10 = 2310,
        M_11 = 2311,
        M_12 = 2312,
        M_13 = 2313,
        M_14 = 2314,
        M_15 = 2315,
        M_16 = 2316,
        M_17 = 2317,
        M_18 = 2318,


        N_01 = 2401,
        N_02 = 2402,
        N_03 = 2403,
        N_04 = 2404,
        N_05 = 2405,
        N_06 = 2406,
        N_07 = 2407,
        N_08 = 2408,
        N_09 = 2409,
        N_10 = 2410,
        N_11 = 2411,
        N_12 = 2412,
        N_13 = 2413,
        N_14 = 2414,
        N_15 = 2415,
        N_16 = 2416,
        N_17 = 2417,
        N_18 = 2418,


        O_01 = 2501,
        O_02 = 2502,
        O_03 = 2503,
        O_04 = 2504,
        O_05 = 2505,
        O_06 = 2506,
        O_07 = 2507,
        O_08 = 2508,
        O_09 = 2509,
        O_10 = 2510,
        O_11 = 2511,
        O_12 = 2512,
        O_13 = 2513,
        O_14 = 2514,
        O_15 = 2515,
        O_16 = 2516,
        O_17 = 2517,
        O_18 = 2518,


        P_01 = 2601,
        P_02 = 2602,
        P_03 = 2603,
        P_04 = 2604,
        P_05 = 2605,
        P_06 = 2606,
        P_07 = 2607,
        P_08 = 2608,
        P_09 = 2609,
        P_10 = 2610,
        P_11 = 2611,
        P_12 = 2612,
        P_13 = 2613,
        P_14 = 2614,
        P_15 = 2615,
        P_16 = 2616,
        P_17 = 2617,
        P_18 = 2618,


        Q_01 = 2701,
        Q_02 = 2702,
        Q_03 = 2703,
        Q_04 = 2704,
        Q_05 = 2705,
        Q_06 = 2706,
        Q_07 = 2707,
        Q_08 = 2708,
        Q_09 = 2709,
        Q_10 = 2710,
        Q_11 = 2711,
        Q_12 = 2712,
        Q_13 = 2713,
        Q_14 = 2714,
        Q_15 = 2715,
        Q_16 = 2716,
        Q_17 = 2717,
        Q_18 = 2718,


        R_01 = 2801,
        R_02 = 2802,
        R_03 = 2803,
        R_04 = 2804,
        R_05 = 2805,
        R_06 = 2806,
        R_07 = 2807,
        R_08 = 2808,
        R_09 = 2809,
        R_10 = 2810,
        R_11 = 2811,
        R_12 = 2812,
        R_13 = 2813,
        R_14 = 2814,
        R_15 = 2815,
        R_16 = 2816,
        R_17 = 2817,
        R_18 = 2818,


        S_01 = 2901,
        S_02 = 2902,
        S_03 = 2903,
        S_04 = 2904,
        S_05 = 2905,
        S_06 = 2906,
        S_07 = 2907,
        S_08 = 2908,
        S_09 = 2909,
        S_10 = 2910,
        S_11 = 2911,
        S_12 = 2912,
        S_13 = 2913,
        S_14 = 2914,
        S_15 = 2915,
        S_16 = 2916,
        S_17 = 2917,
        S_18 = 2918,


        T_01 = 3001,
        T_02 = 3002,
        T_03 = 3003,
        T_04 = 3004,
        T_05 = 3005,
        T_06 = 3006,
        T_07 = 3007,
        T_08 = 3008,
        T_09 = 3009,
        T_10 = 3010,
        T_11 = 3011,
        T_12 = 3012,
        T_13 = 3013,
        T_14 = 3014,
        T_15 = 3015,
        T_16 = 3016,
        T_17 = 3017,
        T_18 = 3018,


        U_01 = 3101,
        U_02 = 3102,
        U_03 = 3103,
        U_04 = 3104,
        U_05 = 3105,
        U_06 = 3106,
        U_07 = 3107,
        U_08 = 3108,
        U_09 = 3109,
        U_10 = 3110,
        U_11 = 3111,
        U_12 = 3112,
        U_13 = 3113,
        U_14 = 3114,
        U_15 = 3115,
        U_16 = 3116,
        U_17 = 3117,
        U_18 = 3118,


        V_01 = 3201,
        V_02 = 3202,
        V_03 = 3203,
        V_04 = 3204,
        V_05 = 3205,
        V_06 = 3206,
        V_07 = 3207,
        V_08 = 3208,
        V_09 = 3209,
        V_10 = 3210,
        V_11 = 3211,
        V_12 = 3212,
        V_13 = 3213,
        V_14 = 3214,
        V_15 = 3215,
        V_16 = 3216,
        V_17 = 3217,
        V_18 = 3218,


        W_01 = 3301,
        W_02 = 3302,
        W_03 = 3303,
        W_04 = 3304,
        W_05 = 3305,
        W_06 = 3306,
        W_07 = 3307,
        W_08 = 3308,
        W_09 = 3309,
        W_10 = 3310,
        W_11 = 3311,
        W_12 = 3312,
        W_13 = 3313,
        W_14 = 3314,
        W_15 = 3315,
        W_16 = 3316,
        W_17 = 3317,
        W_18 = 3318,


        X_01 = 3401,
        X_02 = 3402,
        X_03 = 3403,
        X_04 = 3404,
        X_05 = 3405,
        X_06 = 3406,
        X_07 = 3407,
        X_08 = 3408,
        X_09 = 3409,
        X_10 = 3410,
        X_11 = 3411,
        X_12 = 3412,
        X_13 = 3413,
        X_14 = 3414,
        X_15 = 3415,
        X_16 = 3416,
        X_17 = 3417,
        X_18 = 3418,


        Y_01 = 3501,
        Y_02 = 3502,
        Y_03 = 3503,
        Y_04 = 3504,
        Y_05 = 3505,
        Y_06 = 3506,
        Y_07 = 3507,
        Y_08 = 3508,
        Y_09 = 3509,
        Y_10 = 3510,
        Y_11 = 3511,
        Y_12 = 3512,
        Y_13 = 3513,
        Y_14 = 3514,
        Y_15 = 3515,
        Y_16 = 3516,
        Y_17 = 3517,
        Y_18 = 3518,


        Z_01 = 3601,
        Z_02 = 3602,
        Z_03 = 3603,
        Z_04 = 3604,
        Z_05 = 3605,
        Z_06 = 3606,
        Z_07 = 3607,
        Z_08 = 3608,
        Z_09 = 3609,
        Z_10 = 3610,
        Z_11 = 3611,
        Z_12 = 3612,
        Z_13 = 3613,
        Z_14 = 3614,
        Z_15 = 3615,
        Z_16 = 3616,
        Z_17 = 3617,
        Z_18 = 3618,

        _END_ACCENTS__ = 4000,

        _DEV_CUSTOM__ = 5000,

        // use the enum from 5001 to 5998 to use your own custom enum where necessary
        // do not replace any of the enums above as it may affect the functioning of viperOSK

        _END_DEV_CUSTOM__ = 5999,


    }
}