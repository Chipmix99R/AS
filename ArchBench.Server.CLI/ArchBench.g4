
grammar ArchBench;

//commands    : command
//            ;

command       : HELP
              | START NUMBER
              | STOP
              | INSTALL PATH
              | ENABLE  NUMBER
              | ENABLE  IDENTIFIER
              | DISABLE NUMBER
              | DISABLE IDENTIFIER
              | WITH    identifier SET IDENTIFIER ASSIGNMENT identifier
              | SHOW    identifierOpt
              | EXIT
              ;

identifierOpt : 
              | identifier
              ;

identifier    : NUMBER
              | IDENTIFIER
              ;

HELP        : 'help'
            ;

START       : 'start'
            ;

STOP        : 'stop'
            ;

INSTALL     : 'install'
            ;

ENABLE      : 'enable'
            ;

DISABLE     : 'disable'
            ;

WITH        : 'with'
            ;

SET         : 'set'
            ;

SHOW        : 'show'
            ;

EXIT        : 'exit'
            ;

ASSIGNMENT  : '='
            ;

PRIME       : [']
            ;

IDENTIFIER  : [A-Za-z] ( [A-Za-z] | [0-9] | [_?] )*
            | PRIME [A-Za-z] ( [A-Za-z] | [0-9] | [_?] | [ ] )+ PRIME
            ;

NUMBER      : [0-9]+
            ;

PATH        : [A-Za-z0-9_.:\\/]+
            ;

NEWLINE     : [\n]
            ;

WHITESPACE  : [ \t\r]+ -> skip
            ;

