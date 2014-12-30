lexer grammar SRTLexer;
/*1
00:01:04,773 --> 00:01:07,608
OLD BILBO:
My dear Frodo:

2
00:01:13,156 --> 00:01:14,615
You asked me once...
*/

// Znacznik czasowy. Po jego wykryciu przechodzimy do trybu "TEXT",
// gdzie jako wyswietlany napis traktujemy wszystko do momentu az wykryjemy
// znak podwojnej nowej lini.

TIMESPAN: TIME ARROW TIME -> pushMode(TEXT);
 
TIME: NUMBER NUMBER ':' NUMBER NUMBER ':' NUMBER NUMBER ',' NUMBER NUMBER NUMBER ;

// Numery kolejnych napisow
ID: NUMBER+; 

NUMBER: '0'..'9' ;

// Znaku nowej linii nie przenosimy do parsera
NEWLINE: '\r'? '\n';

ARROW: ' --> ' ;

// Tryb TEXT
mode TEXT;

// Napisy maja wiele linii
CAPTION: LINE+ -> popMode;

// Podwojna nowa linia wraca do normalnygo trybu pracy lexera - tego, gdzie
// rozpoznajemy id i znaczniki czasowe
//ENDEND: END END -> popMode;

// Pojedyncza linia
LINE: ~[\r\n]+ END -> skip;

// Koniec linii dla trybu TEXT
END: '\r'? '\n' -> skip;
