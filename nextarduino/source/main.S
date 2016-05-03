
.extern init
.global main
.text
.include "m2560def.S" 
.include "register.S"
main:
	;Stackinit
	
	ldi temp1, hi8(RAMEND)
	out SPH, temp1
	ldi temp1, lo8(RAMEND)
	out SPL, temp1	

	;C Init
	call init


end:    rjmp end


