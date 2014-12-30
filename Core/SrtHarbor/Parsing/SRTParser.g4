parser grammar SRTParser;

// Odwolanie do lexera, ktory jest w innym pliku
options { tokenVocab = SRTLexer; }

// Na plik sklada sie wiele napisow
file: (subtitle NEWLINE) + subtitle NEWLINE ? EOF;

// Pojedynczy napis
subtitle: ID NEWLINE TIMESPAN CAPTION NEWLINE*;

 