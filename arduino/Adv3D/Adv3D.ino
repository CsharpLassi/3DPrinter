#define X_STEP 22
#define X_DIR  24
#define X_EN   44

#define Y_STEP 36
#define Y_DIR 38
#define Y_EN 48

#define Z_STEP 30
#define Z_DIR 32
#define Z_EN 46

#define E_STEP 33
#define E_DIR 35
#define E_EN 31

#define HOTPIN 2
#define TEMPSEN A0
#define HOTPOWER  255/2

byte mode = 1;
bool startMove = false;

long IstposX = 0;
long IstposY = 0;
long IstposZ = 0;
long IstposE = 0;

long SollposX = 0;
long SollposY = 0;
long SollposZ = 0;
long SollposE = 0;

byte isttemp   =0;
byte solltemp  =0;

int movespeed = 500;

byte facE = 10;
byte facX = 10;
byte facY = 10;

#define TEMPLENGHT 52
byte tempvalues[] = {90  ,95  ,100 ,105 ,110 ,115 ,120 ,125 ,130 ,135 ,140 ,145 ,150 ,155 ,160 ,165 ,170 ,175 ,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,215,220,225};
short tempres[] =   {9044,7760,6684,5778,5011,4243,3806,3333,2926,2577,2275,2015,1789,1591,1423,1274,1143,1028,925,906,887,869,852,843,818,801,785,770,754,739,725,711,697,684,670,657,644,632,620,609,598,587,576,565,555,545,535,525,516,472,432,396};
#include <stdint.h>

void setup() {

  pinMode(HOTPIN,OUTPUT);
  digitalWrite(HOTPIN,LOW);
  //analogWrite(HOTPIN,0);
  
  pinMode(X_STEP,OUTPUT);
  pinMode(X_DIR,OUTPUT);
  pinMode(X_EN,OUTPUT);
  digitalWrite(X_EN,HIGH);
  
  pinMode(Y_STEP,OUTPUT);
  pinMode(Y_DIR,OUTPUT);
  pinMode(Y_EN,OUTPUT);
  digitalWrite(Y_EN,HIGH);
  
  pinMode(Z_STEP,OUTPUT);
  pinMode(Z_DIR,OUTPUT);
  pinMode(Z_EN,OUTPUT);
  digitalWrite(Z_EN,HIGH);

  pinMode(E_STEP,OUTPUT);
  pinMode(E_DIR,OUTPUT);
  pinMode(E_EN,OUTPUT);
  digitalWrite(Z_EN,HIGH);

  Serial.begin(115200);

  startContact();
  UnLockMotors();
}

void startContact()
{
  char rb = 0;
  do
  {
    while (Serial.available() <= 0)
    {
      Serial.print('w');
      delay(500);
    }

    rb = Serial.read();
    
  }while(rb != 's');
  Serial.write('o');
}

byte ReadByte()
{
  byte result = 0;
  while(Serial.available() <= 0);
  result = Serial.read();
  return result;
}

void loop() 
{

  CheckTemp();

  if(startMove)
  {
    switch(mode)
    {
      case 1:
        mode1();
        break;
      case 2:
        mode2();
        break;
    }
  
    if(!CheckPos())
      return;
    else
    {
      startMove = false;
      Serial.write(1);
    }
  }
  while(Serial.available() > 0)
  {
    byte command = ReadByte();
    byte value = ReadByte();
    ParseCommand(command,value);
  }
}

void ParseCommand(byte command, byte value)
{
  if(command == 1) //StartMove
  {
    startMove = true;
  }
  else if(command == 2) //Home
  {
    SollposX =0;
    SollposY =0;
    SollposZ =0;
    startMove = true;
  }
  else if(command == 8) //Home
  {
    mode = value;
  }
  else if (command == 16) //ADD X
  {
    SollposX += value;
  }
  else if (command == 17) //SUB X
  {
    SollposX -= value;
  }
  else if (command == 18) //ADD Y
  {
    SollposY += value;
  }
  else if (command == 19) //SUB Y
  {
    SollposY -= value;
  }
  else if (command == 20) //ADD Z
  {
    SollposZ += value;
  }
  else if (command == 21) //SUB Z
  {
    SollposZ -= value;
  }
  else if (command == 22) //ADD E
  {
    SollposE += value;
  }
  else if (command == 23) //SUB E
  {
    SollposE -= value;
  }
  else if (command == 24) //SetFacE
  {
    facE = value;
  }
  else if (command == 25) //SetFacX
  {
    facX = value;
  }
  else if (command == 26) //SetFacY
  {
    facY = value;
  }
  else if (command == 32) //SET TEMP
  {
    solltemp = value;
  }
  else if (command == 33) //GET TEMP
  {
    Serial.write(isttemp);
  }
  else if (command == 40) //SET DefaultMove
  {
    movespeed = 500;
  }
  else if (command == 41) //SET MoveFirstByte
  {
    movespeed &= 0x00FF;
    
    movespeed |= (value << 8);
  }
  else if (command == 42) //SET MoveLastByte
  {
    movespeed &= 0xFF00;
    movespeed |=value;
  }
}

bool CheckPos()
{
  return  IstposX == SollposX &&
          IstposY == SollposY &&
          IstposZ == SollposZ &&
          IstposE == SollposE;
}

void mode1() //Alles Gleichzeitig
{
  for(int i = 0;i <10;i++)
  {
    if(i < facX)
    {
      if(SollposX>IstposX)
      {
        digitalWrite(X_DIR,LOW);
        digitalWrite(X_STEP,HIGH);
        IstposX++;
      }
      else if(SollposX<IstposX)
      {
        digitalWrite(X_DIR,HIGH);
        digitalWrite(X_STEP,HIGH);
        IstposX--;
      }
    }
    if(i< facY)
    {
      if(SollposY>IstposY)
      {
        digitalWrite(Y_DIR,LOW);
        digitalWrite(Y_STEP,HIGH);
        IstposY++;
      }
      else if(SollposY<IstposY)
      {
        digitalWrite(Y_DIR,HIGH);
        digitalWrite(Y_STEP,HIGH);
        IstposY--;
      }
    }
  
    if(SollposZ>IstposZ)
    {
      digitalWrite(Z_DIR,LOW);
      digitalWrite(Z_STEP,HIGH);
      IstposZ++;
    }
    else if(SollposZ<IstposZ)
    {
      digitalWrite(Z_DIR,HIGH);
      digitalWrite(Z_STEP,HIGH);
      IstposZ--;
    }

    if(i < facE)
    {
       if(SollposE>IstposE)
      {
        digitalWrite(E_DIR,LOW);
        digitalWrite(E_STEP,HIGH);
        IstposE++;
      }
      else if(SollposE<IstposE)
      {
        digitalWrite(E_DIR,HIGH);
        digitalWrite(E_STEP,HIGH);
        IstposE--;
      }
    }

    delayMicroseconds(movespeed);
    digitalWrite(X_STEP,LOW);
    digitalWrite(Y_STEP,LOW);
    digitalWrite(Z_STEP,LOW);
    digitalWrite(E_STEP,LOW);
    delayMicroseconds(movespeed);
  }
}

void mode2() //XYZ
{
  if(SollposX>IstposX)
  {
    digitalWrite(X_DIR,LOW);
    digitalWrite(X_STEP,HIGH);
    IstposX++;
  }
  
  else if(SollposX<IstposX)
  {
    digitalWrite(X_DIR,HIGH);
    digitalWrite(X_STEP,HIGH);
    IstposX--;
  }
  if ( SollposX == IstposX)
  {
    if(SollposY>IstposY)
    {
      digitalWrite(Y_DIR,LOW);
      digitalWrite(Y_STEP,HIGH);
      IstposY++;
    }
    else if(SollposY<IstposY)
    {
      digitalWrite(Y_DIR,HIGH);
      digitalWrite(Y_STEP,HIGH);
      IstposY--;
    }
  }
  
  if ( SollposY == IstposY)
  {
    if(SollposZ>IstposZ)
    {
      digitalWrite(Z_DIR,LOW);
      digitalWrite(Z_STEP,HIGH);
      IstposZ++;
    }
    else if(SollposZ<IstposZ)
    {
      digitalWrite(Z_DIR,HIGH);
      digitalWrite(Z_STEP,HIGH);
      IstposZ--;
    }
  }

  delayMicroseconds(500);
  digitalWrite(X_STEP,LOW);
  digitalWrite(Y_STEP,LOW);
  digitalWrite(Z_STEP,LOW);
  delayMicroseconds(500);
}

void CheckTemp()
{

  static int n = 0;
  
  int temp = analogRead(TEMPSEN);

  if( temp < 900)
  {
    double res = (3000.0)/(1023.0/temp -1);

    
    for(byte i = 0; i < TEMPLENGHT; i++)
    {
      if(tempres[i] < res)
      {
        isttemp = tempvalues[i];
        break;
      }
    }
  }
  else
  {
    isttemp = 0;
  }
   
  if(solltemp > 110 && isttemp < solltemp )
  {
    if(n % 100 == 0)
      digitalWrite(HOTPIN,HIGH);
    else
    {
      n++;
      n%= 100;  
    }
  }
  else
    digitalWrite(HOTPIN,LOW);
}

void LockMotors()
{
  digitalWrite(X_EN,HIGH);
  digitalWrite(Y_EN,HIGH);
  digitalWrite(Z_EN,HIGH);
  digitalWrite(E_EN,HIGH);
}

void UnLockMotors()
{
  digitalWrite(X_EN,LOW);
  digitalWrite(Y_EN,LOW);
  digitalWrite(Z_EN,LOW);
  digitalWrite(E_EN,LOW);
}
