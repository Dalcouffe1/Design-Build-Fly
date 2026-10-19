---
name: Design-Build-Fly Design Review
description: Client-facing engineering design review for a student lift + cruise VTOL, styled as a released engineering drawing.
colors:
  international-orange: "#b8481b"
  international-orange-deep: "#963a14"
  international-orange-wash: "#f7ece6"
  international-orange-rule: "#e4bfae"
  drafting-ink: "#16191d"
  ink-graphite: "#474e57"
  ink-pencil: "#6c737c"
  vellum: "#fbfbfa"
  sheet-white: "#ffffff"
  sheet-tint: "#f4f5f6"
  sheet-tint-2: "#eceef0"
  hairline: "#e3e5e8"
  frame-rule: "#cbcfd4"
  construction-line: "#9ea4ac"
  status-met: "#23744a"
  status-met-wash: "#e7f2ec"
  status-risk: "#94600a"
  status-risk-wash: "#f6efe1"
  status-fail: "#ad2a1f"
  status-fail-wash: "#f8e9e7"
  status-open: "#57489a"
  status-open-wash: "#efedf6"
  status-open-frame: "#a49ec5"
  status-fail-frame: "#c57e79"
  international-orange-dark: "#ec8a58"
  international-orange-deep-dark: "#f4a77f"
  international-orange-wash-dark: "#2b1e17"
  international-orange-rule-dark: "#6b3d27"
  drafting-ink-dark: "#e6e8ea"
  ink-graphite-dark: "#b2b8bf"
  ink-pencil-dark: "#89909a"
  vellum-dark: "#111315"
  sheet-white-dark: "#16191c"
  sheet-tint-dark: "#1c2024"
  sheet-tint-2-dark: "#23282d"
  zebra-dark: "#181b1e"
  hairline-dark: "#2a2f35"
  frame-rule-dark: "#3a4048"
  construction-line-dark: "#5f6670"
  status-met-dark: "#6cc595"
  status-met-wash-dark: "#15261d"
  status-risk-dark: "#dcae5a"
  status-risk-wash-dark: "#2a2213"
  status-fail-dark: "#ef8a7f"
  status-fail-wash-dark: "#321917"
  status-open-dark: "#a99be0"
  status-open-wash-dark: "#221e33"
  status-open-frame-dark: "#636082"
  status-fail-frame-dark: "#96615e"
  series-1-dark: "#7fa8d1"
  series-2-dark: "#6cbdb4"
  series-3-dark: "#dcae5a"
  series-4-dark: "#cf86b4"
  series-5-dark: "#9aaabb"
  series-6-dark: "#a8bf62"
  zebra: "#fafafa"
  series-1: "#2f5d8a"
  series-2: "#2e7f78"
  series-3: "#b7791f"
  series-4: "#8e3b6e"
  series-5: "#5b6b7c"
  series-6: "#6b7f2a"
typography:
  display:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "2.625rem"
    fontWeight: 600
    lineHeight: 1.05
    letterSpacing: "-0.01em"
  display-phone:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "2rem"
    fontWeight: 600
    lineHeight: 1.05
    letterSpacing: "-0.01em"
  figure:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "2rem"
    fontWeight: 600
    lineHeight: 1.05
    fontFeature: "\"tnum\" 1"
  headline:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.875rem"
    fontWeight: 600
    lineHeight: 1.15
    letterSpacing: "-0.005em"
  figure-phone:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.875rem"
    fontWeight: 600
    lineHeight: 1.05
    fontFeature: "\"tnum\" 1"
  headline-phone:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.15
  figure-small:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.15
  title:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.25rem"
    fontWeight: 600
    lineHeight: 1.15
  callout:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.125rem"
    fontWeight: 600
    lineHeight: 1.35
  lede:
    fontFamily: "Source Sans 3, Segoe UI, system-ui, -apple-system, sans-serif"
    fontSize: "1.125rem"
    fontWeight: 400
    lineHeight: 1.62
  subhead:
    fontFamily: "Barlow Semi Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif"
    fontSize: "1rem"
    fontWeight: 600
    lineHeight: 1.25
  body:
    fontFamily: "Source Sans 3, Segoe UI, system-ui, -apple-system, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.6
    fontFeature: "\"tnum\" 1"
  small:
    fontFamily: "Source Sans 3, Segoe UI, system-ui, -apple-system, sans-serif"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.45
    fontFeature: "\"tnum\" 1"
  source:
    fontFamily: "Red Hat Mono, ui-monospace, Consolas, monospace"
    fontSize: "0.8125rem"
    fontWeight: 400
    lineHeight: 1.55
  label:
    fontFamily: "Red Hat Mono, ui-monospace, Consolas, monospace"
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
  button-theme:
    backgroundColor: "transparent"
    textColor: "{colors.ink-graphite}"
    typography: "{typography.label}"
    rounded: "{rounded.hairline}"
    padding: "0 14px"
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
- Ink on vellum, with one International Orange accent used sparingly
- Square geometry (2px or less), no shadows, structure carried by rules
- Condensed technical headings, a humanist sans body, mono labels and citations
- Tabular numerals everywhere
- Full light and dark themes from the same tokens

## Colors

Drafting ink on cool, neutral vellum, with one International Orange accent and a four-colour verdict set. The accent never doubles as a verdict colour.

### Primary
- **International Orange** (#b8481b; dark theme #ec8a58): the aerospace high-visibility orange. It marks position and focus only:
  - the current section number in the contents
  - the active tab underline
  - section eyebrows
  - measurement item IDs (M1, M2 …)
  - link underlines and link hover
  - sort arrows on sorted columns
  - the focus outline
  - the range-slider thumb
- **International Orange Deep** (#963a14): the pressed or darker step of the accent.
- **International Orange Wash** (#f7ece6; dark #2b1e17): the background of the chosen row in a trade table, and text selection.
- **International Orange Rule** (#e4bfae): a soft accent line, reserved for accent-tinted rules.

### Neutral
- **Drafting Ink** (#16191d; dark #e6e8ea): body text, headings, the 2px opening rules and active controls. In light mode it doubles as the fill for pressed buttons.
- **Ink Graphite** (#474e57): secondary text, table headers and button labels.
- **Ink Pencil** (#6c737c): tertiary text, eyebrows, mono labels and citations.
- **Vellum** (#fbfbfa; dark #111315): the page ground.
- **Sheet White** (#ffffff; dark #16191c): surfaces sitting on the vellum, such as tables, title-block cells, cards and the 3D viewer.
- **Sheet Tint** (#f4f5f6) and **Sheet Tint 2** (#eceef0): table-row hover, slider panels and inline code.
- **Hairline** (#e3e5e8): 1px dividers inside blocks.
- **Frame Rule** (#cbcfd4): 1px frames around the title block, the bottom of blocks and control borders.
- **Construction Line** (#9ea4ac): chart axes and unsorted-column arrows.

### Tertiary (verdicts)
- **Status Met** (#23744a, wash #e7f2ec): a requirement met, or a KPI inside its limit.
- **Status Risk** (#94600a, wash #f6efe1): met but thin, or an estimate (EST tags).
- **Status Fail** (#ad2a1f, wash #f8e9e7): a failed requirement or blocking decision. Fail is the only status that also gets a filled wash.
- **Status Open** (#57489a, wash #efedf6): an open decision.
- **Status Open Frame** (#a49ec5) and **Status Fail Frame** (#c57e79): the 1px frames around open and blocking decision cards, and around the blocked banner.

Every colour has a dark-theme partner token with the suffix `-dark`, for example `drafting-ink-dark`. The dark set mirrors the light one role for role, with lighter accents and verdicts so they hold contrast on the dark vellum.

### Chart series
- Six muted categorical colours, **Series 1–6** (#2f5d8a ink blue, #2e7f78 teal, #b7791f ochre, #8e3b6e plum, #5b6b7c slate, #6b7f2a olive), for charts and the concept viewer only. Each has a lighter dark-theme value.

### Named Rules
**The Position-Not-Paint Rule.** International Orange marks where the reader is, or what the reader is acting on. It never fills a large area, never sits behind body text, and never stands in for a verdict.

**The Verdict Colours Rule.** Green, amber, red and violet mean met, risk, fail and open, and nothing else. Never use them for decoration or chart series.

## Typography

**Display Font:** Barlow Semi Condensed (with Arial Narrow, Helvetica Neue, Arial)
**Body Font:** Source Sans 3 (with Segoe UI, system-ui)
**Label/Mono Font:** Red Hat Mono (with ui-monospace, Consolas)

**Character:** The condensed grotesk recalls the lettering on aircraft markings and drawing title blocks. The humanist sans keeps long technical paragraphs easy to read. The mono face gives labels, units, IDs and citations the look of a drawing's notes.

### Hierarchy
The type scale has ten steps: 12, 13, 14, 16, 18, 20, 24, 30, 32 and 42 px. Don't add a step between them.
- **Display** (600, 2.625rem / 42 px, 1.05; 2rem on phones): the report title only.
- **Headline** (600, 1.875rem / 30 px, 1.15; 1.5rem on phones): section titles (h2). Each starts with its section number in mono orange at 0.75em, so the sheet number sits on the same line as the title.
- **Figure** (600, 2rem / 32 px, tabular; 1.875rem on phones): KPI values. **Figure small** (1.5rem) is used for the calculation-chain values.
- **Title** (600, 1.25rem / 20 px, 1.15): subsection titles (h3). A hairline sits above each one, with about 2.75rem before it.
- **Callout** (600, 1.125rem / 18 px): decision questions, figure captions, measurement-box titles and previous/next links.
- **Lede** (400, 1.125rem / 18 px, 1.62): the cover subtitle and the verdict paragraph that opens each section, capped at 74ch.
- **Body** (400, 16 px, 1.6, tabular): running text, capped at 70ch.
- **Small** (400, 0.875rem / 14 px): tables, navigation, KPI qualifiers, buttons, slider labels and the viewer's part label.
- **Source** (mono 400, 0.8125rem / 13 px, 1.55): citation lines prefixed "Source", formulas, measurement IDs and subsection links.
- **Label** (mono 500, 0.75rem / 12 px, uppercase, 0.06–0.1em tracking): title-block field names, KPI labels, table headers, status chips and EST tags. Never set a label smaller than 12 px.

### Named Rules
**The Tabular Numbers Rule.** Every digit on the page uses tabular numerals, so columns of figures line up like a parts list.

**The Readable Floor Rule.** Nothing functional is set below 12 px, and citations are never below 13 px. The client has to be able to read the sources.

**The Lettering Rule.** Condensed display type is for headings and figures only. Running text never uses it, and labels are always mono caps, never small caps in the body face.

## Layout

The page is a single column of report sheets.
- **Width and gutters:** a 1280px maximum, with 24px side gutters (16px at 560px and below).
- **Cover:** the title and subtitle, then the title-block grid across the full width. The title block has five cells (Project, Review date, Stage, Revision, Sources), with the Project cell slightly wider. At 760px and below it becomes two columns, with Project spanning the full width.
- **Desktop (1100px and wider):** a 220px contents column sits on the left and stays in view while scrolling. It lists the numbered sections in plain text, and the current one expands to show its subsections.
- **Below 1100px:** a horizontally scrolling tab strip replaces the contents column and stays fixed to the top while scrolling.
- **One section at a time:** only one section is shown. Each ends with an in-section index and previous/next links.
- **Spacing:**
  - 12px table cells
  - 16–18px panel padding
  - about 26px around the key-figure strip
  - 40–64px before closing blocks such as the measurement list and section index
- **Phones:** charts, tables and code scroll sideways inside their own containers, never the page.

**The Ruled Block Rule.** A major block opens with a 2px Drafting Ink rule and closes with a 1px Frame Rule. This applies to the key-figure strip, data tables, figures, the measurement list and the section index. Don't wrap these blocks in boxes.

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
- **Theme toggle:** a transparent button with a mono label, such as "Theme: system".

### Chips (requirement status)
- **Style:** mono caps (0.66rem) in the status colour, with a 1px border in the same colour and a 2px radius.
- **Fill:** transparent, except Fail, which adds its wash.
- **States:** the four states are met, risk, fail and open.
- **EST tag:** the same shape in Status Risk, used inline to flag an estimate.

### Title block
- **Style:** a five-cell ruled grid on Sheet White. The 1px Frame Rule lines come from 1px gaps over a Frame Rule ground, so the rules stay clean at any column count.
- **Cells:** each has a mono caps field name above a condensed value, for example PROJECT / Design-Build-Fly or REVIEW DATE / 2026-10-13. The project name lives here, not in a label above the title.
- **Placement:** at the foot of the cover, under the subtitle, with a 2px ink rule below the whole cover.

### Key-figure strip
- **Style:** an open strip under a 2px ink rule, not a set of cards.
- **Content:** each figure has a mono caps label, a large condensed value (coloured by verdict where one applies) and a one-line qualifier.
- **Dividers:** hairlines between figures. On phones the strip stacks, with hairlines between rows.

### Decision cards
- **Style:** a full 1px frame in Status Open Frame, or Status Fail Frame when the decision is blocking. There is no side stripe.
- **Header:** the question as a Callout title, led on the same line by a status chip ("Open", or "Blocking" with the Fail wash). The status is never a separate label above the question.
- **Recommendation:** sits in a Sheet Tint panel (a Fail wash when blocking).
- **Citation:** below a hairline.

### Tables
- **Frame:** the table itself carries a 2px ink rule on top and a 1px Frame Rule at the bottom; its scroll wrapper has no border. There are no side borders, no fill and no zebra stripes. Cells are inset 12px, and the first column 8px from the left edge.
- **Header:** mono caps and sticky, on the Vellum ground.
- **Columns and rows:** number columns are right-aligned and tabular. Rows show a Sheet Tint on hover. Sortable headers show ↕, and the sorted column shows an orange arrow.
- **Chosen row:** International Orange Wash.
- **Total row:** a 1px ink rule above it.

### "What the team must measure" list
- **Frame:** a ruled block.
- **Rows:** a mono orange ID in the left column (M1, M2 …), then what to measure, how to measure it, and the failure threshold in Status Fail.

### Section headings
- **Style:** the h2 carries its own section number in mono International Orange, followed by the title. The numbers are real references: the text cites §3 or §12. There is no small label above any heading.

### Navigation
- **Contents column:**
  - A mono "CONTENTS" label sits above a rule.
  - Each section has a right-aligned mono number and a plain-text title.
  - The current section is bold, with an International Orange number.
  - Subsections hang off a 1px rule.
- **Tab strip:** the same content in a sticky horizontal strip, with a 2px orange underline under the active tab.

### Interactive SOLIDWORKS viewer
- **Frame:** a framed Sheet White canvas with a mono caption tag in the corner.
- **Controls:** a Sheet Tint control bar, with buttons following the segmented-control rules.
- **Part label:** hovering over a part shows a small framed label with a condensed part name and a body-text description.
- **Rendering:** CAD colours, hairline feature edges in the theme's ink, and a faint ground grid in Frame Rule.

## Do's and Don'ts

### Do:
- **Do** open each major block with a 2px Drafting Ink rule and close it with a 1px Frame Rule.
- **Do** keep International Orange to position, focus and selection.
- **Do** set labels, IDs, units, field names and citations in Red Hat Mono caps with 0.06–0.1em tracking.
- **Do** use tabular numerals and right-align number columns.
- **Do** put a Source citation under every claim block, and mark estimates with the EST tag.
- **Do** keep radii at 2px or less, and keep controls at least 44px tall.
- **Do** define every colour as a token on `:root`, with dark values under `prefers-color-scheme` and `[data-theme]`.

### Don't:
- **Don't** use box shadows, glows, gradients or blur. The system is flat (user direction: it must not look AI-generated).
- **Don't** use rounded cards, pill badges, dot status icons or coloured side or top stripes on cards.
- **Don't** return to the earlier indigo-on-blue-grey theme or the Archivo/IBM Plex pairing. That's the look this system replaced.
- **Don't** use International Orange as a verdict colour or a large fill.
- **Don't** use zebra stripes or filled header rows in tables.
- **Don't** set running text in the condensed display face.
- **Don't** put a small label or kicker above any heading. Section numbers go inside the heading, and status goes in a chip on the same line.
- **Don't** set functional text below 12 px, or citations below 13 px.
