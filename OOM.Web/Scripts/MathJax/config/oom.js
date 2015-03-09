window.MathJax = {
    config: [],
    styleSheets: [],
    styles: {},
    jax: ["input/AsciiMath", "output/SVG", "output/HTML-CSS", "output/NativeMML", "output/CommonHTML"],
    extensions: ["asciimath2jax.js"],
    preJax: null,
    postJax: null,
    preRemoveClass: "mathjax-preview",
    showProcessingMessages: false,
    messageStyle: "none",
    displayAlign: "center",
    displayIndent: "0",
    delayStartupUntil: "none",
    skipStartupTypeset: true,
    elements: ["math-preview"],
    positionToHash: true,
    showMathMenu: false,
    showMathMenuMSIE: false,
    menuSettings: {
        zoom: "None",        //  when to do MathZoom
        CTRL: false,         //    require CTRL for MathZoom?
        ALT: false,          //    require Alt or Option?
        CMD: false,          //    require CMD?
        Shift: false,        //    require Shift?
        zscale: "200%",      //  the scaling factor for MathZoom
        font: "Auto",        //  what font HTML-CSS should use
        context: "MathJax",  //  or "Browser" for pass-through to browser menu
        mpContext: false,    //  true means pass menu events to MathPlayer in IE
        mpMouse: false,      //  true means pass mouse events to MathPlayer in IE
        texHints: true,      //  include class names for TeXAtom elements
        semantics: false     //  add semantics tag with original form in MathML output
    },
    errorSettings: {
        message: ["[", ["MathProcessingError", "Math Processing Error"], "]"],
        style: { color: "#CC0000", "font-style": "italic" }  // style for message
    },
    asciimath2jax: {
        delimiters: [
          ['`', '`']
        ],
        skipTags: ["script", "noscript", "style", "textarea", "pre", "code", "annotation", "annotation-xml"],
        ignoreClass: "mathjax-ignore",
        processClass: "mathjax-process",
        preview: "AsciiMath"
    },
    AsciiMath: {
        fixphi: true,
        useMathMLspacing: true,
        displaystyle: true,
        decimal: "."
    },
    "HTML-CSS": {
        scale: 100,
        minScaleAdjust: 50,
        availableFonts: ["STIX", "TeX"],
        preferredFont: "TeX",
        webFont: "TeX",
        imageFont: null,
        undefinedFamily: "STIXGeneral,'Arial Unicode MS',serif",
        mtextFontInherit: false,
        EqnChunk: 50,
        EqnChunkFactor: 1.5,
        EqnChunkDelay: 100,
        matchFontHeight: true,
        noReflows: true,
        linebreaks: {
            automatic: false,
            width: "container"
        },
        styles: {},
        tooltip: {
            delayPost: 600,          // milliseconds delay before tooltip is posted after mouseover
            delayClear: 600,         // milliseconds delay before tooltip is cleared after mouseout
            offsetX: 10, offsetY: 5  // pixels to offset tooltip from mouse position
        }
    },
    NativeMML: {
        scale: 100,
        minScaleAdjust: 50,
        matchFontHeight: true,
        styles: {}
    },
    "SVG": {
        scale: 100,
        minScaleAdjust: 50,
        font: "TeX",
        blacker: 10,
        undefinedFamily: "STIXGeneral,'Arial Unicode MS',serif",
        mtextFontInherit: false,
        addMMLclasses: false,
        EqnChunk: 50,
        EqnChunkFactor: 1.5,
        EqnChunkDelay: 100,
        matchFontHeight: true,
        linebreaks: {
            automatic: false,
            width: "container"
        },
        merrorStyle: {
            fontSize: "90%", color: "#C00", background: "#FF8",
            border: "1px solid #C00", padding: "3px"
        },
        styles: {},
        tooltip: {
            delayPost: 600,          // milliseconds delay before tooltip is posted after mouseover
            delayClear: 600,         // milliseconds delay before tooltip is cleared after mouseout
            offsetX: 10, offsetY: 5  // pixels to offset tooltip from mouse position
        }
    },
    MathMenu: {
        delay: 150,
        helpURL: "http://www.mathjax.org/help-v2/user/",
        showRenderer: false,
        showMathPlayer: false,
        showFontMenu: false,
        showContext: false,
        showDiscoverable: false,
        semanticsAnnotations: {
            "TeX": ["TeX", "LaTeX", "application/x-tex"],
            "StarMath": ["StarMath 5.0"],
            "Maple": ["Maple"],
            "ContentMathML": ["MathML-Content", "application/mathml-content+xml"],
            "OpenMath": ["OpenMath"]
        },
        windowSettings: {
            status: "no", toolbar: "no", locationbar: "no", menubar: "no",
            directories: "no", personalbar: "no", resizable: "yes", scrollbars: "yes",
            width: 100, height: 50
        },
        styles: {}
    },
    MathEvents: {
        hover: 500
    },
    MMLorHTML: {
        prefer: {
            MSIE: "MML",
            Firefox: "HTML",
            Opera: "HTML",
            Safari: "HTML",
            Chrome: "HTML",
            other: "HTML"
        }
    }
};