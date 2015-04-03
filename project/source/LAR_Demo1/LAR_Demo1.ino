#include <SPI.h>
#include <Keypad.h>
#include <MFRC522.h>
#define RST_PIN         5 
#define SS_PIN         53
MFRC522 mfrc522(SS_PIN, RST_PIN);
MFRC522::MIFARE_Key key;

const byte ROWS = 4; //four rows
const byte COLS = 4; //four columns
//define the cymbols on the buttons of the keypads
char hexaKeys[ROWS][COLS] = {
  {
    '1','2','3','A'                                    }
  ,
  {
    '4','5','6','B'                                    }
  ,
  {
    '7','8','9','C'                                    }
  ,
  {
    '*','0','#','D'                                    }
};
String listBook[3];
String listIDBook[3];
byte borrowed=0;
byte expired=0;
byte bookIndex=0;
byte idleTime = 0;
byte curStatus = 0;
String input="";
String inputHash="";
String pinNum;
String userName;
String userID;
String toDisplay;
byte invalidCount=0;
byte rowPins[ROWS] = {
  22, 24, 26, 28};
byte colPins[COLS] = {
  30, 32, 34, 36};

Keypad customKeypad = Keypad( makeKeymap(hexaKeys), rowPins, colPins, ROWS, COLS); 
void setup(){
  Serial.begin(9600);
  while(!Serial);
  Serial3.begin(9600);
  while(!Serial3);
  SPI.begin();
  pinMode(8,OUTPUT);
  mfrc522.PCD_Init();
  for(byte i=0; i<6; i++) {
    key.keyByte[i] = 0xFF;
  }
  // clearScreen();
  welcomeScreen();
  Timer1Setup();  
}

ISR(TIMER1_COMPA_vect){//timer1 interrupt ISR
  idleTime++;
  if (idleTime == 180)
  {
    resetAll();
  }
}


void loop(){
  if(mfrc522.PICC_IsNewCardPresent()){
    if(mfrc522.PICC_ReadCardSerial()){
      String data="";
      for(byte i=0;i<4;i++){
        if (mfrc522.uid.uidByte[i]<10)
          data+="00"+String(mfrc522.uid.uidByte[i]);
        else if(mfrc522.uid.uidByte[i]<100)
          data+="0"+String(mfrc522.uid.uidByte[i]);
        else data+=String(mfrc522.uid.uidByte[i]);
      }
      mfrc522.PICC_HaltA();
      idleTime=0;
      scanTagsHandler(data);
    }
  }
  char customKey = customKeypad.getKey();
  if (customKey){
    idleTime=0;
    keypadHandler(customKey);
  }
}

void scanTagsHandler(String tagsID){
  alertSound(1);
  switch (curStatus){
  case 0: 
    firstScan(tagsID);
    // 
    break;
  case 1:
    //do nothing
    break;
  case 2:
    //
    checkNextScan(tagsID);

    break;
  case 3:
    //
    break;
  case 4:
    //
    break;
  case 5:
    //
    break;
  case 6:
    //
    break;
  case 7:
    //
    break;
  case 8:
    //
    break;
  case 9:
    //
    break;

  }
}
void checkNextScan(String tagsID){
  char k = checkTags(tagsID);
  switch(k){
  case '0':
    //do nothing
    break;
  case '1':
    //do nothing
    break;
  case '2':
    {
      char ch = checkBook(tagsID);
      if(ch=='1'){
        if((bookIndex+borrowed)<3){
          String name = getName(tagsID);
          listBook[bookIndex]=name;
          listIDBook[bookIndex]=tagsID;
          bookIndex++;
          clearScreen();
          Serial3.println("1Da them");
          toDisplay=    "2"+name;      
          Serial3.println(toDisplay);
          Serial3.println("3vao danh sach");
          delay(3000);
        }
        else{
          clearScreen();
          Serial3.println("1Ban chi duoc");
          Serial3.println("2muon toi da 3");
          Serial3.println("3sach/tailieu");
          delay(3000);
        }
        menuScreen();
      } 
      else {
        returnBook(tagsID);
        updateStatus();
        menuScreen();
      }
    }
  }
}
void keypadHandler(char inputKey){
  alertSound(1);
  switch (curStatus){

  case 0:
    //Do nothing
    break;
  case 1:
    //input PIN
    inputPinNum(inputKey);
    break;
  case 2:
    if(inputKey=='1'){
      curStatus=3;
      viewInfo();

    } 
    else if (inputKey=='2'){
      curStatus=4;
      confirmBook();    
    } 
    else if(inputKey=='#'){
      resetAll();
    }
    break;
  case 3:
    if(inputKey=='#'){
      menuScreen();
      curStatus=2;
    } 
    break;
  case 4:
    {
      char rr = '0'+bookIndex;
      if(inputKey=='*'){
        doBorrow();
        updateStatus();
        curStatus=2;
      }
      else if(inputKey>='1'&&inputKey<=rr){
        byte index=inputKey-'0';
        bookIndex--;
        for(byte i = index; i<bookIndex; i++){
          listBook[i-1]=listBook[i];
          listIDBook[i-1]=listIDBook[i];
        }
        confirmBook();
      }

    }
    break;
  case 5:
    //
    break;
  case 6:
    //
    break;
  case 7:
    //
    break;
  case 8:
    //
    break;
  case 9:
    //
    break;

  }
}

void alertSound(byte t){
  for(byte i = 0; i<t;i++){
    digitalWrite(8,HIGH);
    delay(100);
    digitalWrite(8,LOW);
  }
}
void updateStatus(){
  String request="";
    request = "6"+userID;
      Serial.print(request);
      while(!Serial.available());
      char k =  Serial.read();
      Serial.flush();
      borrowed=k-'0';
      request = "7"+userID;
      Serial.print(request);
      while(!Serial.available());
      k =  Serial.read();
      Serial.flush();
      expired=k-'0';
    
}
void doBorrow(){
  for(byte i =0;i<bookIndex;i++){
    String request = "8"+userID+listIDBook[i];
    Serial.print(request);
  }
  bookIndex=0;
  menuScreen();
}
void viewInfo(){
  clearScreen();
  toDisplay="11.Sach da muon:"+String(borrowed);
  Serial3.println(toDisplay);
  toDisplay="22.Sach qua han:"+String(expired);
  Serial3.println(toDisplay);
  Serial3.println("4[#]Back");
}

void confirmBook(){
  clearScreen();
  if(bookIndex==0){
    curStatus=2;

    Serial3.println("1Empty!");
    delay(4000);
    menuScreen();   
  }
  else{
    for(byte i=0;i<bookIndex;i++){
      toDisplay = String(i+1)+String(i+1)+'.'+listBook[i];
      Serial3.println(toDisplay);
    }
    Serial3.println("4Huy:1,2,3  [*]OK");
  }

}

void welcomeScreen(){
  clearScreen();
  Serial3.println("2Welcome to LAR");
}

void clearScreen(){
  Serial3.println('c');
}

void firstScan(String tagsID){
  char k = checkTags(tagsID);
  switch(k){
  case '0':
    //do nothing
    break;
  case '1':
    {
      curStatus=1;
      String request = "5"+tagsID;
      Serial.print(request);
      while(!Serial.available());
     /* while(Serial.available()){
        char c = Serial.read();
        pinNum.concat(c);
      }*/
  /*    for(byte i =0;i,4;i++){
        pinNum[i]=Serial.read();
      }*/
      pinNum = Serial.readStringUntil('\n');
      Serial.flush();
      clearScreen();
      userID=tagsID;
      userName = getName(tagsID);
      request = "6"+tagsID;
      Serial.print(request);
      while(!Serial.available());
      char k =  Serial.read();
      Serial.flush();
      borrowed=k-'0';
      request = "7"+tagsID;
      Serial.print(request);
      while(!Serial.available());
      k =  Serial.read();
      Serial.flush();
      expired=k-'0';
      Serial3.println("2PIN:");
      Serial3.println("4C:clear #:cancel");
    }
    break;
  case '2':
    char k = checkBook(tagsID);
    if(k=='1'){
      clearScreen();
      Serial3.println("1Ban vui long");
      Serial3.println("2dang nhap de");
      Serial3.println("3muon sach");
      return;
    } 
    else {
      returnBook(tagsID);
      welcomeScreen();
      return;
    }

    break;
  }
}


void inputPinNum(char inputKey){
  if (inputKey){
    if(inputKey =='#') {
      resetAll();
      return;
    }
    if (inputKey=='C'){
      input="";
      inputHash="";
      Serial3.println("2PIN:    ");
    } 
    else if(inputKey>='0'&&inputKey<='9'){
      input+=inputKey;
      inputHash+="*";
      toDisplay="2PIN:"+inputHash;
      Serial3.println(toDisplay);
    }
  }
  if(input.length()==4){
    boolean c = true;
    for(byte i =0;i<4;i++){
      if (input[i]!=pinNum[i]){
        c=false;
      }
    }
    if(c){
      curStatus=2;      
      menuScreen();
      return;
    }
    else{
      invalidCount++;
      String message="1Nhap sai lan "+String(invalidCount);
      if(invalidCount>3){
        clearScreen();
        Serial3.println("1Tai khoan da bi");
        Serial3.println("2khoa do nhap");
        Serial3.println("3sai nhieu lan");
        delay(5000);
        resetAll();
      }
      Serial3.println(message);
      input="";
      inputHash="";
      Serial3.println("2PIN:    ");
    }
  }
}


void menuScreen(){
  curStatus=2;
  clearScreen();
  toDisplay="1 "+userName;
  Serial3.println(toDisplay);
  Serial3.println("21.Xem thong tin");
  Serial3.println("32.Xac nhan sach");
  Serial3.println("4C:clear #:cancel"); 
}




void resetAll(){
  welcomeScreen();
  curStatus=0;
  input="";
  inputHash="";
  invalidCount=0;
  borrowed=0;
  expired=0;
  bookIndex=0;
  idleTime = 0;
  curStatus = 0;
  //pinNum="";
  userName="";
  userID="";
  invalidCount=0;
  return;
}




String getName(String tagsID){
  String request = "4"+tagsID;
  Serial.print(request);
  while(!Serial.available());
 /* String result="";
  while(Serial.available()){
    char c = Serial.read();
      result.concat(c);
  }*/
  String result = Serial.readStringUntil('\n');
  Serial.flush();
  return result;
}



char checkBook(String tagsID){
  String request = "2"+tagsID;
  Serial.print(request);
  while(!Serial.available());
  char k =  Serial.read();
  Serial.flush();
  return k;   
}



void returnBook(String tagsID){
  String request = '3'+tagsID;  
  Serial.print(request);
  String name  = getName(tagsID);
  clearScreen();
  Serial3.println("1Da tra sach:");
  toDisplay="2"+name;
  Serial3.println(toDisplay);
  delay(3000);
}



char checkTags(String tagsID){
  String request = "1"+tagsID;
  Serial.print(request);
  while(!Serial.available());
  char k = Serial.read();
  Serial.flush();
  return k;
}


void Timer1Setup()
{
  //set timer1 interrupt at 1Hz
  TCCR1A = 0;// set entire TCCR1A register to 0
  TCCR1B = 0;// same for TCCR1B
  TCNT1  = 0;//initialize counter value to 0
  // set compare match register for 1hz increments
  OCR1A = 15624;// = (16*10^6) / (1*1024) - 1 (must be <65536)
  // turn on CTC mode
  TCCR1B |= (1 << WGM12);
  // Set CS10 and CS12 bits for 1024 prescaler
  TCCR1B |= (1 << CS12) | (1 << CS10);  
  // enable timer compare interrupt
  TIMSK1 |= (1 << OCIE1A);

  sei();//allow interrupts
}


















