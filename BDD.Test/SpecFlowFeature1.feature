#language: pt-br

#Frase mais descritiva

Funcionalidade: Dado  que sou um cliente da API
	Gostaria de testar as funcionalidades da API de Usuários

Cenario: Obter usuário por um identificador
	Dado que a url do endpoint é 'http://localhost:58694/api/values/4'
	E o metodo é 'GET'
	Quando chamar o serviço
	Entao statuscode da resposta deverá ser 'OK'

Cenario: Validar o tipo da resposta da api values 4
	Dado que a url do endpoint é 'http://localhost:58694/api/values/4'
	E o metodo é 'GET'
	Quando chamar o serviço
	Entao statuscode da resposta deverá ser 'OK'
	Entao a resposta deve ser "String"

Cenario: Validar o tipo da resposta da api person 10
	Dado que a url do endpoint é 'http://localhost:58694/api/person/10'
	E o metodo é 'GET'
	Quando chamar o serviço
	Entao a resposta deve ser {"nameUIHSASFDUIH": "String", "age": "Date"}

Cenario: Validar o tipo da resposta da api values
	Dado que a url do endpoint é 'http://localhost:58694/api/values'
	E o metodo é 'GET'
	Quando chamar o serviço
	Entao a resposta deve ser ["Integer", "Integer"]

Cenario: Validar o tipo da resposta da api person
	Dado que a url do endpoint é 'http://localhost:58694/api/Person'
	E o metodo é 'GET'
	Quando chamar o serviço
	Entao a resposta deve ser [null, {"nameUIHSASFDUIH": "String", "age": "Date"}]
