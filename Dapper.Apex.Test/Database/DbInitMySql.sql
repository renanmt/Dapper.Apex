USE DapperTest;

CREATE TABLE Model1 (
      `Id` int(11) NOT NULL AUTO_INCREMENT,
      `Prop1` varchar(45) NOT NULL,
      `Prop2` varchar(45) default "DEFAULT VALUE",
      `Prop3` varchar(45) NULL,
      PRIMARY KEY (`Id`)
);

CREATE TABLE Model2 (
      `Model2Id` int(11) NOT NULL AUTO_INCREMENT,
      `Prop1` varchar(45) NOT NULL,
      PRIMARY KEY (`Model2Id`)
);

CREATE TABLE Model3 (
      `Id1` char(36) NOT NULL,
      `Id2` char(36) NOT NULL,
      `Prop1` varchar(45) NOT NULL,
      PRIMARY KEY (`Id1`, `Id2`)
);

CREATE TABLE Model4 (
      `Id` int(11) NOT NULL AUTO_INCREMENT,
      `Prop1` varchar(45) NOT NULL,
      `Prop2` varchar(45) default "DEFAULT VALUE",
      `Prop3` varchar(45) NULL,
      PRIMARY KEY (`Id`)
);