---
name: Design-Build-Fly Design Review
description: Client-facing engineering design review for a student lift + cruise VTOL, set as a released engineering report on a graphite sheet.
colors:
  international-orange: "#ec8a58"
  international-orange-wash: "#3d2a20"
  international-orange-rule: "#7a4a33"
  drafting-ink: "#f1f3f5"
  ink-graphite: "#cdd2d7"
  ink-pencil: "#aeb5bd"
  vellum: "#25282c"
  sheet: "#2e3237"
  sheet-tint: "#363b41"
  hairline: "#3f444b"
  frame-rule: "#545b63"
  construction-line: "#78808a"
  drawing-paper: "#ffffff"
  viewport-stage-top: "#4a525b"
  viewport-stage-bottom: "#22262a"
  viewport-grid: "#77818c"
  status-met: "#6cc595"
  status-met-wash: "#1f3428"
  status-risk: "#dcae5a"
  status-risk-wash: "#3a3020"
  status-fail: "#ef8a7f"
  status-fail-wash: "#43231f"
  status-open: "#a99be0"
  status-open-frame: "#6f6a92"
  status-fail-frame: "#a06a66"
  series-1: "#7fa8d1"
  series-2: "#6cbdb4"
  series-3: "#dcae5a"
  series-4: "#cf86b4"
  series-5: "#9aaabb"
  series-6: "#a8bf62"
  print-bg: "#fbfbfa"
  print-surface: "#ffffff"
  print-surface-2: "#f4f5f6"
  print-line: "#e3e5e8"
  print-line-2: "#cbcfd4"
  print-line-3: "#9ea4ac"
  print-fg: "#16191d"
  print-fg-2: "#474e57"
  print-fg-3: "#6c737c"
  print-accent: "#b8481b"
  print-accent-soft: "#f7ece6"
  print-accent-line: "#e4bfae"
  print-ink-fg: "#ffffff"
  print-ok: "#23744a"
  print-ok-soft: "#e7f2ec"
  print-warn: "#94600a"
  print-warn-soft: "#f6efe1"
  print-bad: "#ad2a1f"
  print-bad-soft: "#f8e9e7"
  print-open: "#57489a"
  print-open-line: "#a49ec5"
  print-bad-line: "#c57e79"
  print-c1: "#2f5d8a"
  print-c2: "#2e7f78"
  print-c3: "#b7791f"
  print-c4: "#8e3b6e"
  print-c5: "#5b6b7c"
  print-c6: "#6b7f2a"
typography:
  display:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "2.625rem"
    fontWeight: 700
    lineHeight: 1.05
    letterSpacing: "-0.01em"
  display-phone:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "2rem"
    fontWeight: 700
    lineHeight: 1.05
    letterSpacing: "-0.01em"
  chapter:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "2rem"
    fontWeight: 600
    lineHeight: 1.15
    letterSpacing: "-0.005em"
  headline:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.875rem"
    fontWeight: 600
    lineHeight: 1.15
    letterSpacing: "-0.005em"
  chain-value:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.15
    fontFeature: "\"tnum\" 1"
  title:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.25rem"
    fontWeight: 600
    lineHeight: 1.15
  figure:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.25rem"
    fontWeight: 600
    lineHeight: 1.2
    fontFeature: "\"tnum\" 1"
  callout:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.125rem"
    fontWeight: 600
    lineHeight: 1.35
  lede:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.125rem"
    fontWeight: 400
    lineHeight: 1.62
  subhead:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1rem"
    fontWeight: 600
    lineHeight: 1.25
  body:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.6
    fontFeature: "\"tnum\" 1"
  summary:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.875rem"
    fontWeight: 600
    lineHeight: 1.3
  small:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.45
    fontFeature: "\"tnum\" 1"
  label:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.8125rem"
    fontWeight: 600
    lineHeight: 1.3
  source:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.8125rem"
    fontWeight: 400
    lineHeight: 1.55
  chart:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "12px"
    fontWeight: 400
    fontFeature: "\"tnum\" 1"
  tag:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.75rem"
    fontWeight: 600
    lineHeight: 1
    letterSpacing: "0.04em"
rounded:
  none: "0px"
  hairline: "2px"
spacing:
  gutter-phone: "16px"
  gutter: "24px"
  cell: "12px"
  panel: "18px"
  block: "22px"
  scroll-margin: "64px"
  chapter: "96px"
components:
  button-segment:
    backgroundColor: "{colors.sheet}"
    textColor: "{colors.ink-graphite}"
    rounded: "{rounded.hairline}"
    padding: "0 16px"
    height: "44px"
  button-segment-active:
    backgroundColor: "{colors.drafting-ink}"
    textColor: "{colors.vellum}"
    rounded: "{rounded.hairline}"
    padding: "0 16px"
    height: "44px"
  button-link:
    backgroundColor: "{colors.drafting-ink}"
    textColor: "{colors.vellum}"
    typography: "{typography.subhead}"
    rounded: "{rounded.hairline}"
    padding: "0 16px"
    height: "44px"
  button-link-quiet:
    backgroundColor: "transparent"
    textColor: "{colors.drafting-ink}"
    typography: "{typography.subhead}"
    rounded: "{rounded.hairline}"
    padding: "0 16px"
    height: "44px"
  chip-met:
    backgroundColor: "transparent"
    textColor: "{colors.status-met}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "4px 7px"
  chip-risk:
    backgroundColor: "transparent"
    textColor: "{colors.status-risk}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "4px 7px"
  chip-fail:
    backgroundColor: "{colors.status-fail-wash}"
    textColor: "{colors.status-fail}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "4px 7px"
  chip-open:
    backgroundColor: "transparent"
    textColor: "{colors.status-open}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "4px 7px"
  decision-card:
    backgroundColor: "{colors.sheet}"
    textColor: "{colors.ink-graphite}"
    rounded: "{rounded.hairline}"
    padding: "16px 20px 14px"
  key-parameter-row:
    backgroundColor: "transparent"
    textColor: "{colors.drafting-ink}"
    typography: "{typography.figure}"
    padding: "9px 8px"
  table-head:
    backgroundColor: "{colors.vellum}"
    textColor: "{colors.drafting-ink}"
    typography: "{typography.label}"
    padding: "10px 12px"
  table-row-chosen:
    backgroundColor: "{colors.international-orange-wash}"
    textColor: "{colors.drafting-ink}"
    padding: "9px 12px"
  disclosure-summary:
    backgroundColor: "transparent"
    textColor: "{colors.drafting-ink}"
    typography: "{typography.summary}"
    height: "44px"
  running-head:
    backgroundColor: "{colors.vellum}"
    textColor: "{colors.ink-pencil}"
    typography: "{typography.source}"
    padding: "8px 24px"
  step-block:
    backgroundColor: "{colors.sheet}"
    textColor: "{colors.drafting-ink}"
    rounded: "{rounded.hairline}"
    padding: "14px 16px 6px"
  step-check-done:
    backgroundColor: "{colors.status-met-wash}"
    textColor: "{colors.status-met}"
    typography: "{typography.summary}"
    rounded: "{rounded.hairline}"
    padding: "0 14px"
    height: "44px"
---

# Design System: Design-Build-Fly Design Review

## Overview

**Creative North Star: "The Released Engineering Report"**

The review reads like a released, numbered engineering report, not a web page. It opens on a report title page (mast line, title, a one-sentence document reference, a revision record and signature lines), then a contents page grouped into three Parts, then numbered chapters read top to bottom. A running head, chapter and figure numbers generated from page order, and live § cross-references make it behave like a bound document that renumbers itself. Its two companion documents, the build manual and the test plan, are the same report set in the same style.

The page is dense on purpose. Numbers sit inline with their units, a Source line closes every claim block, and structure is carried by rules rather than boxes: a 2px ink rule opens each major block and 1px hairlines divide what is inside. Everything is one serif face, STIX Two Text, on a single graphite theme: a mid-grey ground, panels stepping lighter, off-white text. Colour is held back for meaning. International Orange marks where the reader is and what is selected, and four status colours mark verdicts. The client has to trust what they are reading before they approve the spend, and the restraint is what earns that.

Detail shows in alignment, tabular numerals and consistent rules, never in ornament: no shadows, no gradients outside the CAD stage, no rounded cards.

**Key Characteristics:**
- A report title page with a revision record and signature lines, closed by a 2px ink rule
- One graphite theme: off-white ink on a mid-grey ground with lighter grey panels; print switches to white paper
- One serif face, STIX Two Text, for everything; hierarchy from size, weight and rules
- International Orange for position and selection only
- Square geometry (2px or less), no shadows, structure carried by ruled blocks
- Tabular numerals everywhere
- Chapter, subsection, figure and measurement numbers generated from page order, never typed

## Colors

Off-white drafting ink on a graphite sheet, one International Orange accent and a four-colour verdict set. The accent never doubles as a verdict colour.

### Primary
- **International Orange**: the aerospace high-visibility orange. It marks position, focus and selection:
  - link underlines and link hover, and the hovered contents entry
  - the focus outline on every control, table and chart scroller
  - sort arrows on a sorted column
  - the range-slider thumb
  - the highlighted part in the CAD viewer
  - the chosen design (C0) in the design-iterations chart
  - reference marks the reader acts on: measurement IDs (M5.1 …) and work-package step numbers (WP7.1 …) on the companion pages
- **International Orange Wash**: the chosen row in a trade table, and text selection.
- **International Orange Rule**: the quiet link underline in the shop-file list and on the companion pages.

### Neutral
- **Drafting Ink**: body text, headings, the 2px opening rules, and the fill of pressed segments and primary link buttons (with Vellum text).
- **Ink Graphite**: secondary text, decision-card body, button labels, figure and chart labels.
- **Ink Pencil**: tertiary text: chapter and subsection numbers, requirement IDs, citations, the running head.
- **Vellum**: the page ground, sticky table headers and the running head.
- **Sheet**: surfaces one step up: decision cards, figures, chain boxes, step blocks, the viewer bar, part buttons.
- **Sheet Tint**: the next step up: table-row hover, the recommendation panel, slider panels, segmented-control trays, inline code.
- **Hairline**: 1px dividers inside blocks.
- **Frame Rule**: 1px closing rules under blocks, control borders and the running-head rule.
- **Construction Line**: chart axes, the viewer frame and unsorted-column arrows.
- **Drawing Paper**: the white behind SOLIDWORKS drawing sheets and in the sheet lightbox, so drawings read as printed.
- **Viewport Stage Top / Bottom** and **Viewport Grid**: the graded CAD stage and its ground grid (55% opacity), the one gradient in the system.

There is one theme. The ground is a mid-grey, not black, and each layer above it steps lighter (Vellum, Sheet, Sheet Tint), so panels separate by tone as well as by rules. Text is off-white, never pure white, and every text and verdict colour holds at least 4.5:1 on the layer it sits on.

### Tertiary (verdicts)
- **Status Met** (with its wash): a requirement met, a KPI inside its limit, a step marked done on the companion pages.
- **Status Risk** (with its wash): met but thin, or an estimate (EST tags); Safety and Abort fields in step blocks.
- **Status Fail** (with its wash): a failed requirement, a blocking decision, a measurement's failure threshold. Fail is the only chip that also gets a filled wash.
- **Status Open**: an open decision.
- **Status Open Frame** and **Status Fail Frame**: the 1px frames around open and blocking decision cards.

### Chart series
- Six muted categorical colours, **Series 1–6** (ink blue, teal, ochre, plum, slate, olive), for charts only. The iterations chart uses Series 1 for designs that meet every requirement and Series 5 for those that break R1; ranked-out designs are an outlined Sheet Tint bar with a dashed Construction Line.

### Print
- The `print-*` colours are the paper palette: white paper, near-black ink, a deeper orange, and darker verdict and series colours. They replace the graphite values only inside `@media print`.

### Named Rules
**The Position-Not-Paint Rule.** International Orange marks where the reader is, what is selected, or the reference mark they act on. It never fills a large area, never sits behind body text, and never stands in for a verdict.

**The Verdict Colours Rule.** Green, amber, red and violet mean met, risk, fail and open, and nothing else. Never use them for decoration or chart series.

**The Never-Colour-Alone Rule.** Every status shown by colour is also stated in words or form. A status cell carries a short note in its own colour, a chosen option is bold and named, and a decision card's status is a worded chip.

## Typography

**Typeface:** STIX Two Text (with Times New Roman, Times, Liberation Serif, serif) for everything: title, headings, body, labels, data, charts and citations (client direction, 2026-10-21). It loads from Google Fonts as a variable font, roman and italic, weights 400–700; Times New Roman is the offline fallback. There is no sans and no mono face. The `--f-mono` token is kept, set to the same serif, so a data face could return without touching every rule.

**Character:** One serif face throughout, as in a printed engineering report. Hierarchy comes from size, weight and the ruled blocks, not from changing typeface. Weights are 400 for reading text, 600 for headings, labels and values, 700 for the report title and total rows; control labels in segmented bars and dialogs use 500. Italic appears on the signature lines.

### Hierarchy
The type scale has ten steps: 12, 13, 14, 16, 18, 20, 24, 30, 32 and 42 px. Don't add a step between them.
- **Display** (700, 42 px, 1.05, −0.01em; 32 px on phones): the report title only, capped at 18ch.
- **Chapter** (600, 32 px, 1.15): chapter headings in the report. The chapter number sits inside the heading in Ink Pencil, then the title.
- **Headline** (600, 30 px, 1.15): the closing "What we ask" heading and chapter headings on the companion pages.
- **Chain value** (600, 24 px, tabular): values in the back-calculation chain.
- **Title** (600, 20 px, 1.15): subsection headings (h3), numbered "4.2" in Ink Pencil, each under a hairline with about 2.75rem above; also the contents heading.
- **Figure** (600, 20 px, 1.2, tabular, right-aligned): key-parameter values.
- **Callout** (600, 18 px, 1.35): decision questions, figure captions (led by a bold "Figure n.k."), measurement-box titles, step headings.
- **Lede** (400, 18 px, 1.62): the verdict paragraph that opens each chapter (74ch) and the cover subtitle.
- **Subhead** (600, 16 px, 1.25): h4, contents entries, the CAD part-label name, link-button labels.
- **Body** (400, 16 px, 1.6, tabular): running text, capped at 70ch (72ch on companion pages).
- **Summary** (600, 14 px, 1.3): disclosure summaries, key-parameter names, the mast line, Part headings in the contents, step-field names.
- **Small** (400, 14 px, 1.45): tables, KPI qualifiers, the requirement grid, file descriptions, tooltips, control buttons.
- **Label** (600, 13 px, 1.3, sentence case): table headers, status chips, viewer group captions, chain labels.
- **Source** (400, 13 px, 1.55): citation lines prefixed "Source", formulas, the running head, signature captions, `small` notes.
- **Chart** (400, 12 px, tabular): all SVG chart text; group heads in charts are 600.
- **Tag** (600, 12 px, 0.04em, uppercase): the EST estimate tag only.

### Named Rules
**The Tabular Numbers Rule.** Every digit on the page uses tabular numerals, so columns of figures line up like a parts list.

**The Readable Floor Rule.** Nothing functional is set below 12 px, and citations are never below 13 px. The client has to be able to read the sources.

**The Report Numbering Rule.** Chapters, subsections, figures and measurements are numbered the way an engineering report numbers them: chapter 5, subsection 5.2, Figure 5.3, measurement M5.1. The numbers come from page order at load, never typed by hand, and every § cross-reference fills from the same order, so inserting a chapter renumbers everything after it.

**The One Face Rule.** Every element, including data, code and chart text, is set in STIX Two Text. Separate levels with size, weight and rules, never with a second family or small caps.

## Layout

The report reads one chapter at a time, like turning to a tab in a bound report. A binder bar holds the three Parts and the chapters of the current Part; print shows every chapter in order.
- **Width and gutters:** a 1040px maximum page, centred, with 24px side gutters (16px at 560px and below). Prose stops at 70ch; tables, figures and the 3D model use the full column.
- **Cover:** a report title page: a mast line (organisation and programme left, document number and issue right) over a Frame Rule hairline, the title and subtitle, a one-sentence document reference naming the document (bold), issue, date and the generated design revision, then the revision record beside the approval signature lines. Below 760px the approval block stacks under the record. A 2px ink rule closes the cover.
- **Contents page:** grouped into three Parts (The decision, The design, Build and fly), one column per Part (stacking on narrow screens). Each Part has a small numeral line ("Part I" to "Part III") above its name, then a 2px ink rule and its chapters as 44px rows: number in Ink Pencil, title in Subhead. Companion pages use a two-level contents: chapters, with their work packages as wrapped links underneath, and appendices (revision notes, inconsistency logs) lettered and placed last.
- **Binder:** a sticky bar under the cover. Top row: three Part tabs ("Part I" numeral over the Part name) in a three-column grid on a 2px ink rule; the current Part fills with Drafting Ink. Second row: the current Part's chapters as a sideways-scrolling strip of 44px links, number in Ink Pencil, the open chapter underlined with a 2px International Orange rule. Anchors carry a 120px scroll margin so the bar never covers a target.
- **Routing:** each chapter is its own hash route; links and § cross-references open the chapter that holds their target and move focus to it, and the browser's Back button returns. The home route shows the cover, contents, overview and closing; any other chapter hides them.
- **Previous / next:** each chapter ends with two framed cards (Previous left, Next right; stacked on phones) under a 2px ink rule.
- **Chapters:** each opens with a 2px ink rule and its numbered heading, closed by a Frame Rule.
- **Summary first:** each chapter leads with its verdict paragraph, key parameters and charts. Supporting detail sits behind named disclosures: long tables ("Show the full table · 43 rows"), the facts behind each decision card, history.
- **Overview order:** verdict and key parameters, the general-arrangement drawing, decisions needed (the first card is the explicit approval ask), requirement status, the consolidated measurement list, and a glossary.
- **Closing:** the report ends with the ask and a dated list of next steps under a 2px ink rule.
- **Phones:** tables, code and wide charts scroll sideways inside their own named, focusable containers, never the page; the two horizontal bar charts redraw at the container width with each label above its bar instead of scrolling.

**The Ruled Block Rule.** A major block opens with a 2px Drafting Ink rule and closes with a 1px Frame Rule: chapter headings, key parameters, data tables, figures, measurement boxes, contents Parts and the closing. Lists inside a block (requirements, files, measurements, glossary, next steps) open with a 1px Frame Rule and divide with hairlines. Do not wrap these blocks in boxes.

**The Summary-First Rule.** A reader gets each chapter's answer from its first screen. Long tables and supporting facts fold behind a disclosure that names what it holds; charts and verdicts never do.

## Elevation & Depth

The system is flat. It has no shadows at all (`--shadow: none`). Depth comes from tone and drawing conventions instead: Sheet surfaces sit on the Vellum ground, Sheet Tint sits on Sheet, 2px ink rules open a block and hairlines subdivide it, and pressed controls fill with Drafting Ink. The running head and sticky table headers separate from content with a 1px rule on the Vellum ground.

**The Flat Sheet Rule.** Nothing floats. If an element needs to stand apart, give it a rule, a frame, a tonal step or an ink fill, never a shadow, glow or blur.

## Shapes

Every corner is square or nearly so: a 2px radius on controls, cards, chips, step blocks and the viewer, and 0 on rules and tables. Frames are 1px; the only heavier line is the 2px ink rule that opens a block and closes the cover. The chosen concept card is marked with a doubled ink outline (a 1px border plus a 1px inset outline). Disclosures open with a small drawn chevron, not a glyph. There are no pills, circles, dots or status lights.

**The Square Corner Rule.** Radius never exceeds 2px.

## Components

### Buttons and segmented controls
- **Character:** precise and restrained, like the switches on a test rig. Every control is at least 44px tall.
- **Segmented control:** a Sheet Tint tray with a 1px border and 3px inset holding Sheet segments 3px apart; hover steps the segment to Sheet Tint, pressed (`aria-pressed="true"`) fills with Drafting Ink and Vellum text. Pressed controls never use orange.
- **Link button:** a Drafting Ink fill with Vellum text in Subhead, for links out to the companion documents; the quiet variant is transparent with a Drafting Ink frame and hovers to Sheet Tint.
- **Secondary buttons** (Close, Try again, part buttons, Mark done): Sheet or transparent with a Frame Rule border; hover darkens the border to Drafting Ink.

### Chips (status)
- **Style:** Label type in the status colour, 1px border in the same colour, 2px radius, transparent fill except Fail, which adds its wash.
- **States:** met, risk, fail, open; decision cards add "Open" and "Blocking" chips on the question line.
- **EST tag:** the Tag style in Status Risk, inline, to flag an estimate.

### Title page (cover)
- **Document reference:** one sentence under the subtitle; it replaces any grid of fact tiles.
- **Revision record:** a ruled table (issue, date, description) between Frame Rule lines, captioned "Revision record".
- **Approval:** three signature lines (Prepared, Checked, Approved for the client): a rule with the signer in italic above and the role and date in Source below. Unsigned lines stay blank.

### Key parameters
- **Style:** a ruled block, one parameter per row: name (Summary), value (Figure, right-aligned, coloured by verdict where one applies) and a one-line qualifier (Small). A data-sheet table, not a strip of big numbers. On phones, name and value share a line with the qualifier below.

### Decision cards
- **Style:** a Sheet card with a full 1px frame in Status Open Frame, or Status Fail Frame when blocking. No side stripe.
- **Header:** the question in Callout, led on the same line by its status chip.
- **Recommendation:** a Sheet Tint panel (a Fail wash when blocking). Facts sit in a named disclosure; the citation sits below a hairline.
- **Order:** the first card in the overview is the explicit approval ask.

### Requirement status
- A count line ("6 met · 5 at risk · 9 open of 20 requirements") over a grid sorted by status, each row the requirement ID in Ink Pencil, its short text and its chip; the full table sits in a disclosure below.

### Measurements and glossary
- **Measurement boxes:** a ruled block per chapter titled in Callout; each row has a section-scoped ID (M5.1) in International Orange, what to measure, how (Ink Graphite) and the failure threshold in Status Fail.
- **Measurement list:** the overview's consolidated list of every ID, linked to its chapter.
- **Glossary:** a ruled two-column term list, term in 600, definition in Ink Graphite.

### Disclosures
- A 44px summary row between hairlines in Summary type, with a drawn chevron that turns on open. The summary names what it holds; the line above it carries the conclusion.

### Tables
- **Frame:** a 2px ink rule on top and a 1px Frame Rule at the bottom; no side borders, fill or zebra. Cells are inset 12px, the first column 8px. The scroll wrapper is a named, focusable region with a 640px height cap (none on phones).
- **Header:** Label type, sticky, on Vellum. A sortable header is a full-cell button whose arrow shows the state: ↕ in Construction Line, an orange ↑ or ↓ when sorted.
- **Rows:** numbers right-aligned and tabular; Sheet Tint on hover; the chosen row in International Orange Wash; status cells in their wash with the value and a short note in the status colour; a total row in 700 under a 1px ink rule.

### Charts and figures
- **Frame:** a figure is a ruled block on Sheet, captioned in Callout and numbered "Figure n.k" from page order.
- **Drawing:** SVG with Chart type, Construction Line axes and dashed hairline grids, Series colours for data.
- **Design iterations:** three ranked groups (meets every requirement, breaks R1, ranked out) with group heads in 600; C0 is named in bold and marked with a short orange bar.
- **Drag build-up and section polar:** horizontal bars and a polar plot in the same vocabulary; the two bar charts switch to label-above-bar at phone width.
- **Drawing sheets:** SOLIDWORKS sheets on Drawing Paper with a 1px Frame Rule border, numbered as figures; selecting one opens a lightbox dialog with a title bar and a Close button.

### Interactive SOLIDWORKS viewer
- **Frame:** a 1px Construction Line frame around the graded viewport stage, so the white airframe never sits on white.
- **Controls:** a Sheet bar under the stage with two captioned segmented groups, View (Isometric, Top, Side, Front) and Display (X-ray, Explode, Edges); the model name and revision sit at the right end (below on phones).
- **Parts list:** a disclosure split into parts and fasteners, one 44px button each; hover or focus highlights the part in International Orange and shows a framed label (Subhead name, Small description).
- **Behaviour:** renders on demand, loads only when its chapter is reached, shows a short offline note and "Try again" when the library fails (controls hidden), and on touch screens takes gestures only after a "Tap to rotate the model" tap. Camera moves and explode jump instantly under reduced motion.

### Flow visualisation
- **Frame:** one Sheet block with a Construction Line frame: a control bar, the canvas, and a foot holding the legend and a one-line readout.
- **Controls:** borderless segmented groups inside the bar (the frame is the only box): view (Wing section, 2-D CFD / Whole aircraft, 3-D); for the section, angle of attack and Speed / Pressure; for the aircraft, camera presets. Play/Pause sits at the right end (wraps on phones).
- **Section view:** the solver's field drawn as a diverging ramp on the dark ground (cool #6f9fd0 below the reference, the ground-adjacent #32373d at it, warm #e09a5c above), the aerofoil in Sheet Tint with an Ink Graphite outline, white particle traces fading over ten steps, and labels on Sheet plates kept inside the canvas. The free stream runs left to right; the aerofoil pitches to the chosen angle.
- **Aircraft view:** painter-sorted lifting-surface panels in Construction Line grey with streamlines on the same ramp through a mid grey (#5c646e) so far-field lines stay visible; drag or arrow keys orbit.
- **Behaviour:** data parse and animation start only when the figure scrolls into view and stop when it leaves; under reduced motion the section shows static streamlines and the aircraft view no particles. The ramp colours are data encodings, not palette roles.

### Shop files
- A ruled list: file link (600, International Orange Rule underline) and a Small description, stacking on phones.

### Companion document pages
- **Shell:** the same mast line, title, running head and two-level contents as the report.
- **Step blocks:** each work-instruction step is a Sheet block with a Frame Rule border; its heading carries the step number (WP7.1) in International Orange, then a field list (Parts, Fasteners, Tools, Procedure, Check, Safety …): field names in Summary at 7.5rem, Check names in Status Met, Safety and Abort names in Status Risk; multi-action procedures become a numbered list.
- **Mark done:** a 44px button per step that persists in the browser only; done fills it with Status Met wash and frames the step in Status Met.
- **Appendices:** revision notes and inconsistency logs move to lettered appendices at the end.

### Navigation and skip link
- Following a contents or § link moves focus to the target heading. A skip link (Drafting Ink tag, Vellum text) sits above the page until focused.

### Windows High Contrast
- Text, frames and controls follow system colours. Chart areas and the 3D canvas opt out (`forced-color-adjust: none`) and keep their own palette, so encodings survive.

### Print
- Printing switches to the `print-*` paper palette, hides interactive-only parts (viewer, sliders, segmented controls, parts list, contents, running head, skip link, touch gates), starts each chapter on a new page, unfolds scrolling tables, and avoids breaking figures, decisions, measurement boxes, tables and key parameters across pages.

## Do's and Don'ts

### Do:
- **Do** open each major block with a 2px Drafting Ink rule and close it with a 1px Frame Rule.
- **Do** keep International Orange to position, focus, selection and reference marks.
- **Do** set every element in STIX Two Text; separate levels with size, weight and rules.
- **Do** use tabular numerals and right-align number columns.
- **Do** put a Source line under every claim block, and mark estimates with the EST tag.
- **Do** generate chapter, figure, measurement and § numbers from page order.
- **Do** keep radii at 2px or less, and controls at least 44px tall.
- **Do** define every colour as a token on `:root`; print overrides them with the `print-*` palette.

### Don't:
- **Don't** use box shadows, glows or blur, or gradients outside the CAD stage. The system is flat (user direction: it must not look AI-generated).
- **Don't** use rounded cards, pill badges, dot status icons or coloured side or top stripes on cards.
- **Don't** add a light screen theme or drop the ground to near-black; the graphite steps carry the contrast.
- **Don't** bring back a sans or mono face (Barlow, Source Sans, Red Hat Mono, Archivo, IBM Plex); the report is one serif.
- **Don't** use International Orange as a verdict colour or a large fill.
- **Don't** use zebra stripes or filled header rows in tables.
- **Don't** put a small label or kicker above any heading. Numbers go inside the heading, and status goes in a chip on the same line.
- **Don't** set functional text below 12 px, or citations below 13 px.
- **Don't** add a type step between the ten on the scale.
