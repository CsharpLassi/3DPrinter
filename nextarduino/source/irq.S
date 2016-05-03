.global irq

.extern irq_timer_temperature

#.include "m2560def.S" 
.section .irq, "ax" 
jmp main ; Reset Handler
jmp irqhandle ; External Interrupt Request 0
jmp irqhandle ; External Interrupt Request 1
jmp irqhandle ; External Interrupt Request 2
jmp irqhandle ; External Interrupt Request 3
jmp irqhandle ; External Interrupt Request 4
jmp irqhandle ; External Interrupt Request 5
jmp irqhandle ; External Interrupt Request 6
jmp irqhandle ; External Interrupt Request 7
jmp irqhandle ; Pin Change Interrupt Request 0
jmp irqhandle ; Pin Change Interrupt Request 1
jmp irqhandle ; Pin Change Interrupt Request 2
jmp irqhandle ; Watchdog Time-out Interrupt
jmp irqhandle ; Timer/Counter2 Compare Match A
jmp irqhandle ; Timer/Counter2 Compare Match B
jmp irqhandle ; Timer/Counter2 Overflow
jmp irqhandle ; Timer/Counter1 Capture Event
jmp irqhandle ; Timer/Counter1 Compare Match A
jmp irqhandle ; Timer/Counter1 Compare Match B
jmp irqhandle ; Timer/Counter1 Compare Match C
jmp irqhandle ; Timer/Counter1 Overflow
jmp irqhandle ; Timer/Counter0 Compare Match A
jmp irqhandle ; Timer/Counter0 Compare Match B
jmp irqhandle ; Timer/Counter0 Overflow
jmp irqhandle ; SPI Serial Transfer Complete
jmp irqhandle ; USART0 Rx Complete
jmp irqhandle ; USART0 Data Register Empty
jmp irqhandle ; USART0 Tx Complete
jmp irqhandle ; Analog Comparator
jmp irqhandle ; ADC Conversion Complete
jmp irqhandle ; EEPROM Ready
jmp irqhandle ; Timer/Counter3 Capture Event
jmp irqhandle ; Timer/Counter3 Compare Match A
jmp irqhandle ; Timer/Counter3 Compare Match B
jmp irqhandle ; Timer/Counter3 Compare Match C
jmp irqhandle ; Timer/Counter3 Overflow
jmp irqhandle ; USART1 Rx Complete
jmp irqhandle ; USART1 Data Register Empty
jmp irqhandle ; USART1 Tx Complete
jmp irqhandle ; 2-wire Serial Interface
jmp irqhandle ; Store Program Memory Ready
jmp irqhandle ; Timer/Counter4 Capture Event
jmp irqhandle ; Timer/Counter4 Compare Match A
jmp irqhandle ; Timer/Counter4 Compare Match B
jmp irqhandle ; Timer/Counter4 Compare Match C
jmp irqhandle ; Timer/Counter4 Overflow
jmp irqhandle ; Timer/Counter5 Capture Event
jmp irqhandle ; Timer/Counter5 Compare Match A
jmp irqhandle ; Timer/Counter5 Compare Match B
jmp irqhandle ; Timer/Counter5 Compare Match C
jmp irqhandle ; Timer/Counter5 Overflow
jmp irqhandle ; USART2 Rx Complete
jmp irqhandle ; USART2 Data Register Empty
jmp irqhandle ; USART2 Tx Complete
jmp irqhandle ; USART3 Rx Complete
jmp irqhandle ; USART3 Data Register Empty
jmp irqhandle ; USART3 Tx Complete

.text
irqhandle:
	reti

