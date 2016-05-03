BUILD = build/

SOURCE = source/

OBJECTS := $(patsubst $(SOURCE)%.S,$(BUILD)%.S.o,$(wildcard $(SOURCE)*.S))  $(patsubst $(SOURCE)%.c,$(BUILD)%.c.o,$(wildcard $(SOURCE)*.c)) 

TARGET = output.hex

MCU = atmega2560

CONF = config/avrdude.conf

PORT = /dev/ttyACM0

MAP = output.map

MAINFILE = main


all: $(TARGET)

rebuild: all

$(TARGET): $(BUILD)output.elf
	avr-objdump -h -S "$(BUILD)output.elf" > "$(MAP)"
	avr-objcopy --output-target=ihex $(BUILD)output.elf $@

#-Wl,--start-group -Wl,-lm  -Wl,--end-group -Wl,--gc-sections -mrelax
$(BUILD)output.elf : $(OBJECTS)
	@echo Building target: $@
	#avr-ar rcs $@.a $^ 
	avr-ld $^ --section-start .irq=0x00 -Ttext 0x72  -m avr6 --relax   -o $@
	@echo Finished building target: $@

$(BUILD)%.S.o: $(SOURCE)%.S $(BUILD)
	@echo Building file: $< 
	 avr-as -c -mmcu=$(MCU) -I "def" -I "$(SOURCE)" -o $@ "$<"

$(BUILD)%.c.o: $(SOURCE)%.c $(BUILD)
	@echo Building file: $< 
	avr-gcc -Os -c -mmcu=$(MCU) -D __AVR_ATmega2560__  -I "include" -o $@ "$<"

$(BUILD):
	mkdir $@

install:
	avrdude -C $(CONF) -v -p $(MCU) -carduino -P $(PORT) -b115200 -D -Uflash:w:$(TARGET):i 


clean :
	-rm -rf $(BUILD)
