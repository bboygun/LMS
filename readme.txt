ѧϰMySQLʱд��ͼ��ݹ���ϵͳ
���ݿ�������Ϣ������Main.cs
�����ϢΪ
	administartor:
		ID,CHAR(7),primary key;
		pasword,VARCHAR(32);
		name,VARCHAR(45);
		contact,VARCHAR(45);
	book:
		booknumber,CHAR(13),primary key;
		type,VARCHAR(45)��
		bookname,VARCHAR(45);
		publishing,VARCHAR(45);
		year,INT;
		author,VARCHAR(45);
		price,DECIMAL(7,2);
		amount,INT;
		stock,INT;
	librarycard:
		cardnumber,CHAR(10),primary key;
		name,VARCHAR(45);
		department,VARCHAR(45);
		type,VARCHAR(20);
	record:
		booknumber,CHAR(13);
		cardnumber,CHAR(10);
		borrowdate,DATE;
		returndate,DATE;
		handler,CHAR(7);
ʹ��ǰ����administrator��������д���һ������Ա