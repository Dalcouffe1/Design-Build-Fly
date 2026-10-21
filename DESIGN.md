---
name: Design-Build-Fly Design Review
description: Client-facing engineering design review for a student lift + cruise VTOL, styled as a released engineering drawing.
colors:
  print-bg: "#fbfbfa"
  print-surface: "#ffffff"
  print-surface-2: "#f4f5f6"
  print-surface-3: "#eceef0"
  print-zebra: "#fafafa"
  print-line: "#e3e5e8"
  print-line-2: "#cbcfd4"
  print-line-3: "#9ea4ac"
  print-fg: "#16191d"
  print-fg-2: "#474e57"
  print-fg-3: "#6c737c"
  print-accent: "#b8481b"
  print-accent-2: "#963a14"
  print-accent-soft: "#f7ece6"
  print-accent-line: "#e4bfae"
  print-accent-fg: "#ffffff"
  print-ink: "#16191d"
  print-ink-fg: "#ffffff"
  print-ok: "#23744a"
  print-ok-soft: "#e7f2ec"
  print-warn: "#94600a"
  print-warn-soft: "#f6efe1"
  print-bad: "#ad2a1f"
  print-bad-soft: "#f8e9e7"
  print-open: "#57489a"
  print-open-soft: "#efedf6"
  print-open-line: "#a49ec5"
  print-bad-line: "#c57e79"
  print-c1: "#2f5d8a"
  print-c2: "#2e7f78"
  print-c3: "#b7791f"
  print-c4: "#8e3b6e"
  print-c5: "#5b6b7c"
  print-c6: "#6b7f2a"
  print-band: "#ffffff"
  print-band-line: "#cbcfd4"
  international-orange: "#ec8a58"
  international-orange-deep: "#f4a77f"
  international-orange-wash: "#3d2a20"
  international-orange-rule: "#7a4a33"
  drafting-ink: "#f1f3f5"
  ink-graphite: "#cdd2d7"
  ink-pencil: "#aeb5bd"
  vellum: "#25282c"
  sheet-white: "#2e3237"
  sheet-tint: "#363b41"
  sheet-tint-2: "#3f454c"
  hairline: "#3f444b"
  frame-rule: "#545b63"
  construction-line: "#78808a"
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
  status-open-wash: "#2f2a45"
  status-open-frame: "#6f6a92"
  status-fail-frame: "#a06a66"
  zebra: "#2a2d32"
  series-1: "#7fa8d1"
  series-2: "#6cbdb4"
  series-3: "#dcae5a"
  series-4: "#cf86b4"
  series-5: "#9aaabb"
  series-6: "#a8bf62"
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
  figure:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "2rem"
    fontWeight: 600
    lineHeight: 1.05
    fontFeature: "\"tnum\" 1"
  headline:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.875rem"
    fontWeight: 600
    lineHeight: 1.15
    letterSpacing: "-0.005em"
  figure-phone:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.875rem"
    fontWeight: 600
    lineHeight: 1.05
    fontFeature: "\"tnum\" 1"
  headline-phone:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.15
  figure-small:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.15
  title:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "1.25rem"
    fontWeight: 600
    lineHeight: 1.15
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
  small:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.45
    fontFeature: "\"tnum\" 1"
  source:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.8125rem"
    fontWeight: 400
    lineHeight: 1.55
  label:
    fontFamily: "STIX Two Text, Times New Roman, Times, Liberation Serif, serif"
    fontSize: "0.75rem"
    fontWeight: 500
    lineHeight: 1.3
    letterSpacing: "0.07em"
rounded:
  none: "0px"
  hairline: "2px"
spacing:
  gutter-phone: "16px"
  gutter: "24px"
  cell: "12px"
  panel: "18px"
  block: "26px"
  section: "64px"
components:
  button-segment:
    backgroundColor: "{colors.sheet-white}"
    textColor: "{colors.ink-graphite}"
    rounded: "{rounded.hairline}"
    padding: "0 16px"
    height: "44px"
  button-segment-active:
    backgroundColor: "{colors.drafting-ink}"
    textColor: "{colors.sheet-white}"
    rounded: "{rounded.hairline}"
    padding: "0 16px"
    height: "44px"
  chip-met:
    backgroundColor: "transparent"
    textColor: "{colors.status-met}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "4px 6px"
  chip-fail:
    backgroundColor: "{colors.status-fail-wash}"
    textColor: "{colors.status-fail}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "4px 6px"
  title-block-cell:
    backgroundColor: "{colors.sheet-white}"
    textColor: "{colors.drafting-ink}"
    padding: "10px 14px 12px"
  decision-card:
    backgroundColor: "{colors.sheet-white}"
    textColor: "{colors.ink-graphite}"
    rounded: "{rounded.hairline}"
    padding: "16px 20px 14px"
  table-head:
    backgroundColor: "{colors.sheet-white}"
    textColor: "{colors.ink-graphite}"
    typography: "{typography.label}"
    padding: "10px 12px"
  table-row-chosen:
    backgroundColor: "{colors.international-orange-wash}"
    textColor: "{colors.drafting-ink}"
    padding: "9px 12px"
---

# Design System: Design-Build-Fly Design Review

## Overview

**Creative North Star: "The Drawing Title Block"**

The review reads like a released engineering drawing, not a web page. Its cover is a title block: a ruled grid of cells for review date, stage, revision and sources, sitting under a heavy ink rule. Everything below follows the drawing sheet: black drafting ink on near-white vellum, square corners, hairline frames, and condensed technical lettering for headings. Authority comes from precision, so the system has no gradients, shadows or rounded cards, and nothing that reads as a marketing site or a generated template.

The page is dense on purpose. It carries numbers inline with units, cites a source under every claim, and puts structure in rules rather than boxes. A 2px ink rule opens each major block (key figures, tables, figures, the measurement list) and 1px hairlines divide what's inside. Colour is held back for meaning: International Orange marks where you are, and the four status colours mark verdicts. The client has to be able to trust what they're reading before they approve the spend, and the restraint is what earns that.

The page is precise and restrained. Detail shows in alignment, tabular numerals and consistent rules, never in ornament.

**Key Characteristics:**
- A ruled title-block cover under a 2px ink rule
- Off-white ink on a graphite ground, with one International Orange accent used sparingly
- Square geometry (2px or less), no shadows, structure carried by rules
- Condensed technical headings, a humanist sans body, mono labels and citations
- Tabular numerals everywhere
- One dark theme: a mid-grey ground with lighter grey panels, not black and white (client direction, 2026-10-21)

## Colors

Drafting ink on cool, neutral vellum, with one International Orange accent and a four-colour verdict set. The accent never doubles as a verdict colour.

### Primary
- **International Orange** (#ec8a58): the aerospace high-visibility orange. It marks position and focus only:
  - the current section number in the contents
  - the active tab underline
  - section eyebrows
  - measurement item IDs (M1, M2 …)
  - link underlines and link hover
  - sort arrows on sorted columns
  - the focus outline
  - the range-slider thumb
- **International Orange Deep** (#f4a77f): the pressed or darker step of the accent.
- **International Orange Wash** (#3d2a20): the background of the chosen row in a trade table, and text selection.
- **International Orange Rule** (#7a4a33): a soft accent line, reserved for accent-tinted rules.

### Neutral
- **Drafting Ink** (#f1f3f5): body text, headings, the 2px opening rules and active controls. It is also the fill for pressed buttons, with Vellum text.
- **Ink Graphite** (#cdd2d7): secondary text, table headers and button labels.
- **Ink Pencil** (#aeb5bd): tertiary text, eyebrows, mono labels and citations.
- **Vellum** (#25282c): the page ground.
- **Sheet White** (#2e3237): surfaces sitting on the vellum, such as tables, title-block cells, cards and the 3D viewer's control bar.
- **Sheet Tint** (#363b41) and **Sheet Tint 2** (#3f454c): table-row hover, slider panels and inline code.
- **Hairline** (#3f444b): 1px dividers inside blocks.
- **Frame Rule** (#545b63): 1px frames around the title block, the bottom of blocks and control borders.
- **Construction Line** (#78808a): chart axes and unsorted-column arrows.

### Tertiary (verdicts)
- **Status Met** (#6cc595, wash #1f3428): a requirement met, or a KPI inside its limit.
- **Status Risk** (#dcae5a, wash #3a3020): met but thin, or an estimate (EST tags).
- **Status Fail** (#ef8a7f, wash #43231f): a failed requirement or blocking decision. Fail is the only status that also gets a filled wash.
- **Status Open** (#a99be0, wash #2f2a45): an open decision.
- **Status Open Frame** (#6f6a92) and **Status Fail Frame** (#a06a66): the 1px frames around open and blocking decision cards, and around the blocked banner.

There is one theme. The ground is a mid-grey (Vellum #25282c), not black, and each layer above it steps lighter (Sheet White #2e3237, Sheet Tint #363b41, Sheet Tint 2 #3f454c), so panels separate by tone as well as by rules. Text is off-white, never pure white, and every text and verdict colour holds at least 4.5:1 on the layer it sits on.

### Chart series
- Six muted categorical colours, **Series 1–6** (#7fa8d1 ink blue, #6cbdb4 teal, #dcae5a ochre, #cf86b4 plum, #9aaabb slate, #a8bf62 olive), for charts only.

### Named Rules
**The Position-Not-Paint Rule.** International Orange marks where the reader is, or what the reader is acting on. It never fills a large area, never sits behind body text, and never stands in for a verdict.

**The Verdict Colours Rule.** Green, amber, red and violet mean met, risk, fail and open, and nothing else. Never use them for decoration or chart series.

**The Never-Colour-Alone Rule.** Every status shown by colour is also stated in words or form. A status cell carries a short note ("below margin", "within ±15 %") in its own status colour. A chosen option is bold and named in its column header. Paired chart series differ in lightness or outline, not only hue.

## Typography

**Typeface:** STIX Two Text (with Times New Roman, Times, Liberation Serif, serif) for everything: title, headings, body, labels, data and citations (client direction, 2026-10-21: a Times-like serif that reads better than Times New Roman). STIX Two descends from the Times-style STIX fonts made for scientific and technical publishing, and loads from Google Fonts as a variable font (weights 400–700, roman and italic); Times New Roman is the fallback offline.

**Character:** One serif face throughout, as in a printed engineering report. Hierarchy comes from size, weight (400, 600 and 700) and the ruled blocks, not from changing typeface. Its figures are tabular, so the Tabular Numbers Rule holds without a separate data face.

### Hierarchy
The type scale has ten steps: 12, 13, 14, 16, 18, 20, 24, 30, 32 and 42 px. Don't add a step between them.
- **Display** (700, 2.625rem / 42 px, 1.05; 2rem on phones): the report title only.
- **Headline** (600, 1.875rem / 30 px, 1.15; 1.5rem on phones): section titles (h2). Each starts with its section number in mono orange at 0.75em, so the sheet number sits on the same line as the title.
- **Figure** (600, 2rem / 32 px, tabular; 1.875rem on phones): KPI values. **Figure small** (1.5rem) is used for the calculation-chain values.
- **Title** (600, 1.25rem / 20 px, 1.15): subsection titles (h3). A hairline sits above each one, with about 2.75rem before it.
- **Callout** (600, 1.125rem / 18 px): decision questions, figure captions, measurement-box titles and previous/next links.
- **Lede** (400, 1.125rem / 18 px, 1.62): the cover subtitle and the verdict paragraph that opens each section, capped at 74ch.
- **Body** (400, 16 px, 1.6, tabular): running text, capped at 70ch.
- **Small** (400, 0.875rem / 14 px): tables, navigation, KPI qualifiers, buttons, slider labels and the viewer's part label.
- **Source** (mono 400, 0.8125rem / 13 px, 1.55): citation lines prefixed "Source", formulas, measurement IDs and subsection links.
- **Label** (body 600, 0.8125rem / 13 px, sentence case): table headers, key-parameter names, status chips and control captions. Title-block field names are the one place labels stay uppercase (body 600, 0.75rem, 0.04em tracking), as on a drawing title block. Never set a label smaller than 12 px.
- **Data** (STIX Two Text, the `--f-mono` token): file paths, citations, measurement IDs (M1 …) and code use the same face as the body; the token is kept so a data face can return without touching every rule.

### Named Rules
**The Tabular Numbers Rule.** Every digit on the page uses tabular numerals, so columns of figures line up like a parts list.

**The Readable Floor Rule.** Nothing functional is set below 12 px, and citations are never below 13 px. The client has to be able to read the sources.

**The Report Numbering Rule.** Sections, subsections and figures are numbered the way an engineering report numbers them: section 4, subsection 4.2, Figure 4.3. The numbers come from the page structure at load, never typed by hand, so inserting a subsection renumbers everything after it.

**The Lettering Rule.** Condensed display type is for headings and figures only. Running text never uses it, and labels are always mono caps, never small caps in the body face.

## Layout

The page is one continuous engineering report, read top to bottom like a printed one. There is no sidebar and no tab switching; every section is on the page at once.
- **Width and gutters:** a 1040px maximum page, centred, with 24px side gutters (16px at 560px and below). Prose stops at 70ch; tables, figures and the 3D model use the full column.
- **Cover:** a report title page: a mast line (organisation, document number and issue), the title and subtitle, a one-sentence document reference, then the revision record beside the approval signature lines.
- **Contents page:** a numbered, two-column table of contents under the cover (one column on phones), each entry a 44px row with the section number in pencil and the title in the condensed face.
- **Running head:** once the contents page scrolls away, a thin bar fixed to the top shows the document number and issue, the section being read, and a link back to Contents, like the header line on a report page. It slides in, never covers content (sections and anchors carry a 64px scroll margin), and drops the document number on phones.
- **Chapters:** each section opens with a 2px ink rule and its number and title (2rem), with 96px of space before it, so the report reads in chapters.
- **Summary first:** each section leads with its verdict paragraph, key figures and charts. Supporting detail sits behind disclosures that say what they hold: any table over ten rows ("Show the full table · 43 rows"), the facts and sources of each decision card, and history such as the v0.2 changes.
- **Overview order:** verdict and key parameters, then the decisions needed from the client, then requirement status as a count line and a status grid, then workstream status, then the change history.
- **Phones:** charts, tables and code scroll sideways inside their own containers, never the page.

**The Ruled Block Rule.** A major block opens with a 2px Drafting Ink rule and closes with a 1px Frame Rule. This applies to the key-parameter table, data tables, figures, the measurement list and each chapter heading. Do not wrap these blocks in boxes.

**The Summary-First Rule.** A reader gets each section's answer from its first screen. Long tables and supporting facts fold behind a labelled disclosure; charts and verdicts never do.

## Elevation & Depth

The system is flat. It has no shadows at all (`--shadow: none`). Depth comes from the drawing conventions instead:
- Sheet White surfaces sit on the Vellum ground.
- 2px ink rules open a block and hairlines subdivide it.
- Pressed controls fill with Drafting Ink.

Sticky elements (the contents column and tab strip) separate from the content with a 1px rule, not a shadow.

**The Flat Sheet Rule.** Nothing floats. If an element needs to stand apart, give it a rule, a frame or an ink fill, never a shadow, glow or blur.

## Shapes

Every corner is square or nearly so: a 2px radius on controls, cards, chips and the viewer, and 0 on rules and tables. Frames are 1px. The only heavier line is the 2px ink rule that opens a block, plus the 2px border under the cover.

The selected concept card is marked with a doubled ink outline (a 1px border plus a 1px inset outline), like a boxed callout on a drawing.

There are no pills, circles, dots or status lights.

**The Square Corner Rule.** Radius never exceeds 2px.

## Components

### Buttons and segmented controls
- **Character:** precise and restrained, like the switches on a test rig.
- **Shape:** near-square (2px), at least 44px tall. A segmented control is a Sheet Tint tray with a 1px Frame Rule border and 3px inset, holding Sheet White segments 3px apart.
- **Default:** a Sheet White fill with Ink Graphite labels in the body face.
- **Hover:** the border darkens to Drafting Ink and the label to full ink.
- **Pressed or active** (`aria-pressed="true"`): a Drafting Ink fill with white text. Pressed buttons never use orange.

### Chips (requirement status)
- **Style:** mono caps (0.66rem) in the status colour, with a 1px border in the same colour and a 2px radius.
- **Fill:** transparent, except Fail, which adds its wash.
- **States:** the four states are met, risk, fail and open.
- **EST tag:** the same shape in Status Risk, used inline to flag an estimate.

### Title page (cover)
- **Mast line:** organisation and programme in Ink Graphite at 14 px on the left, the document number and issue on the right, over a Frame Rule hairline.
- **Document reference:** one sentence under the subtitle naming the document number (bold), issue, date and design revision. It replaces a grid of fact tiles.
- **Revision record:** a ruled table (issue, date, description) between Frame Rule lines, captioned "Revision record".
- **Approval:** three signature lines (Prepared, Checked, Approved for the client), each a 1px rule with the signer in italic above it and the role and date below. Unsigned lines stay blank.
- **Phones:** below 760px the approval block stacks under the revision record. A 2px ink rule closes the cover.

### Secondary detail
- **Style:** history and supporting detail that the reader does not need to decide sits behind a disclosure (`details.more`): a 44px summary row between hairlines, closed by default. The one-line summary above it carries the conclusion.

### Drawing sheets
- **Style:** SOLIDWORKS sheets shown as numbered figures on a white sheet with a 1px Frame Rule border; selecting one opens it at full size in a dialog with a title bar and a Close button.

### Key-parameter table
- **Style:** a ruled table under a 2px ink rule, one parameter per row: name (body 600), value (condensed 600, 1.25rem, right-aligned, coloured by verdict where one applies) and a one-line qualifier. It is a data-sheet table, not a strip of big numbers.
- **Phones:** name and value on one line, qualifier below.

### Decision cards
- **Style:** a full 1px frame in Status Open Frame, or Status Fail Frame when the decision is blocking. There is no side stripe.
- **Header:** the question as a Callout title, led on the same line by a status chip ("Open", or "Blocking" with the Fail wash). The status is never a separate label above the question.
- **Recommendation:** sits in a Sheet Tint panel (a Fail wash when blocking).
- **Citation:** below a hairline.

### Tables
- **Frame:** the table itself carries a 2px ink rule on top and a 1px Frame Rule at the bottom; its scroll wrapper has no border. There are no side borders, no fill and no zebra stripes. Cells are inset 12px, and the first column 8px from the left edge.
- **Header:** mono caps and sticky, on the Vellum ground. A sortable header holds a full-cell button (no visual change) whose arrow shows the state: ↕ unsorted, an orange ↑ or ↓ when sorted.
- **Columns and rows:** number columns are right-aligned and tabular. Rows show a Sheet Tint on hover. Sortable headers show ↕, and the sorted column shows an orange arrow.
- **Chosen row:** International Orange Wash.
- **Status cells:** a Status Met, Risk or Fail wash with the value in the status colour and a short note in words, also in the status colour.
- **Total row:** a 1px ink rule above it.

### "What the team must measure" list
- **Frame:** a ruled block.
- **Rows:** a mono orange ID in the left column (M1, M2 …), then what to measure, how to measure it, and the failure threshold in Status Fail.

### Section headings
- **Style:** the h2 carries its own section number in mono International Orange, followed by the title. The numbers are real references: the text cites §3 or §12. There is no small label above any heading.

### Skip link
- **Style:** a Drafting Ink tag with white mono text, hidden above the page until focused, then shown at the top left. Activating it moves focus to the current section heading.

### Windows High Contrast
- **Style:** text, frames and controls follow the system colours. Chart drawing areas and 3D canvases opt out (`forced-color-adjust: none`) and keep their own token palette on Sheet White, so the chart encodings survive.

### Print
- **Style:** printing switches to a paper palette (the `print-*` colours: white paper, dark ink, the original light verdict colours) so a printed copy needs no dark fill; printing drops the interactive-only parts (viewers, sliders, segmented controls, parts list, skip link, touch gates), shows every section, keeps the static renders, and avoids breaking figures, decisions, tables and measurement lists across pages.

### Navigation
- **Contents page:** numbered two-column list under the cover; each row links to its chapter, and following a link moves keyboard focus to the chapter heading.
- **Running head:** fixed thin bar (document and issue, current section, Contents link); hidden while the cover and contents are on screen.
- **Requirement status grid:** a count line ("6 met · 5 at risk · 9 open of 20 requirements") over a grid of rows, each with the requirement ID in pencil, its short text and its status chip; the full table sits in a disclosure below.

### Interactive SOLIDWORKS viewer
- **Frame:** a 1px Construction Line frame around a graded viewport stage (Viewport Stage Top #4a525b to Viewport Stage Bottom #22262a), like a CAD graphics area, so the white airframe never sits on white.
- **Controls:** a Sheet White bar under the stage with two labelled button groups, View (Iso, Top, Side, Front) and Display (X-ray, Explode, Edges), each a segmented strip (Sheet Tint track, 3px inset, Sheet White buttons) with Drafting Ink for pressed toggles. The model name and revision sit at the right end of the bar (under the groups on phones).
- **Part label:** hovering over a part shows a small framed label with a condensed part name and a body-text description.
- **Rendering:** CAD colours, hairline feature edges in the theme's ink, and a ground grid in Viewport Grid (#77818c) at 55% opacity.
- **Size:** the canvas height is capped by both the width breakpoints and the screen height (`clamp(260px, 100svh − 140px, 540px)`), so a landscape phone shows the model and its controls together.
- **Rendering on demand:** the viewer draws frames only while something moves (orbit, damping, camera move, explode) and stops when idle.
- **Offline:** if the library cannot load, the frame shows only a short note pointing to the static renders and a "Try again" button. The control bar and the parts list are hidden, so no control is ever shown without something behind it.
- **Loading:** the 3D library loads only when the CAD section opens. Until then a mono "Loading the 3D model…" caption sits on the empty frame.
- **Touch:** on touch screens the canvas ignores gestures until the reader taps a "Tap to rotate the model" tag (mono caps on Sheet White, Frame Rule border). Tapping outside the viewer, or scrolling it out of view, turns gestures off again, so swipes always scroll the page.
- **Parts list:** a "Parts in the model" disclosure under the controls holds one 44px button per part. Hover or keyboard focus highlights that part in International Orange and shows its label.

## Do's and Don'ts

### Do:
- **Do** open each major block with a 2px Drafting Ink rule and close it with a 1px Frame Rule.
- **Do** keep International Orange to position, focus and selection.
- **Do** set every element in STIX Two Text; separate levels with size, weight and rules.
- **Do** use tabular numerals and right-align number columns.
- **Do** put a Source citation under every claim block, and mark estimates with the EST tag.
- **Do** keep radii at 2px or less, and keep controls at least 44px tall.
- **Do** define every colour as a token on `:root`; there is one theme, so there are no light overrides.

### Don't:
- **Don't** use box shadows, glows, gradients or blur. The system is flat (user direction: it must not look AI-generated).
- **Don't** use rounded cards, pill badges, dot status icons or coloured side or top stripes on cards.
- **Don't** return to the earlier indigo-on-blue-grey theme or the Archivo/IBM Plex pairing. That's the look this system replaced.
- **Don't** add a light theme or drop the ground to near-black; the graphite steps are what give the page its contrast.
- **Don't** use International Orange as a verdict colour or a large fill.
- **Don't** use zebra stripes or filled header rows in tables.
- **Don't** set running text in the condensed display face.
- **Don't** put a small label or kicker above any heading. Section numbers go inside the heading, and status goes in a chip on the same line.
- **Don't** set functional text below 12 px, or citations below 13 px.
