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
  
  if( SollposX != IstposX || SollposY != IstposY)
  {
    long sdx =  SollposX - IstposX;
    long sdy =  SollposY - IstposY;
    long sde =  SollposE - IstposE;
    
    bresenham( sdx,  sdy, sde);
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

  delayMicroseconds(movespeed);
  //digitalWrite(X_STEP,LOW);
  //digitalWrite(Y_STEP,LOW);
  digitalWrite(Z_STEP,LOW);
  digitalWrite(E_STEP,LOW);
  delayMicroseconds(movespeed);
  
    /*
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
    */    
}

int sgn(int x){
  return (x > 0) ? 1 : (x < 0) ? -1 : 0;
}



//Bresenham
void bresenham(long sdx, long sdy,long sde)
{
   long  x, y, t, dx, dy, incx, incy, ince, pdx, pdy, ddx, ddy, es, el, err;
   long facE = (sde *10) / (abs(sdy) + abs(sdx));
   long se = 0;
   long sc = 0;

   if(sde == 0)
      ince = 0;
   else if( sde > 0)
      ince = 1;
   else
      ince = -1;
   
/* Entfernung in beiden Dimensionen berechnen */
   dx = sdx;
   dy = sdy;

/* Vorzeichen des Inkrements bestimmen */
   incx = sgn(dx);
   incy = sgn(dy);
   if(dx<0) dx = -dx;
   if(dy<0) dy = -dy;

/* feststellen, welche Entfernung größer ist */
   if (dx>dy)
   {
      /* x ist schnelle Richtung */
      pdx=incx; pdy=0;    /* pd. ist Parallelschritt */
      ddx=incx; ddy=incy; /* dd. ist Diagonalschritt */
      es =dy;   el =dx;   /* Fehlerschritte schnell, langsam */
   } 
   else
   {
      /* y ist schnelle Richtung */
      pdx=0;    pdy=incy; /* pd. ist Parallelschritt */
      ddx=incx; ddy=incy; /* dd. ist Diagonalschritt */
      es =dx;   el =dy;   /* Fehlerschritte schnell, langsam */
   }

/* Initialisierungen vor Schleifenbeginn */
   err = el/2;

/* Pixel berechnen */
   for(t=0; t<el; ++t) /* t zaehlt die Pixel, el ist auch Anzahl */
   {
      /* Aktualisierung Fehlerterm */
      err -= es;
      if(err<0)
      {
          /* Fehlerterm wieder positiv (>=0) machen */
          err += el;

          sc += abs(ddx) + abs(ddy);

          se = sc < facE ? ince : 0;
          sc = sc < 10 ? sc : 0;

          /* Schritt in langsame Richtung, Diagonalschritt */
          movexyz(ddx,ddy,se);
      } 
      else
      {
          sc += abs(pdx) + abs(pdy);

          se = sc < facE ? ince : 0;
          sc = sc < 10 ? sc : 0;
        
          /* Schritt in schnelle Richtung, Parallelschritt */
          movexyz(pdx,pdy,se);
      }
   }

  //Extruder fertig setzen
  IstposE = SollposE;
   
} /* gbham() */


void movexyz(long x, long y , long e)
{
  if(x > 0)
  {
    digitalWrite(X_DIR,LOW);
    digitalWrite(X_STEP,HIGH);
    IstposX++;
  }
  else if(x < 0)
  {
    digitalWrite(X_DIR,HIGH);
    digitalWrite(X_STEP,HIGH);
    IstposX--;
  }
  
  if(y > 0)
  {
    digitalWrite(Y_DIR,LOW);
    digitalWrite(Y_STEP,HIGH);
    IstposY++;
  }
  else if(y < 0)
  {
    digitalWrite(Y_DIR,HIGH);
    digitalWrite(Y_STEP,HIGH);
    IstposY--;
  }

  if(e > 0)
  {
    digitalWrite(E_DIR,LOW);
    digitalWrite(E_STEP,HIGH);
    IstposE++;
  }
  else if(e < 0)
  {
    digitalWrite(E_DIR,HIGH);
    digitalWrite(E_STEP,HIGH);
    IstposE--;
  }

  delayMicroseconds(movespeed);
  digitalWrite(X_STEP,LOW);
  digitalWrite(Y_STEP,LOW);
  digitalWrite(E_STEP,LOW);
  delayMicroseconds(movespeed);
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
