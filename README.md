# s3627805_s3829221_a2
Internet banking web application for WDT assignment 2 in flexible term 2021

---

- s3627805 ZipengZheng
- s3829221 Chenyu Xiao

Complete up to DI part

---

- Validation:
Validations are implemented on both server side and client side. Server-side validations are conducted through data annotations in models and Fluent API in the context class. Client-side validations are implemented by using combo boxes and restricting user from inputing invalid values. The use of combo box ensures the input is valid and greatly increases convinience of filling the forms.

---

- Records:
Record is implemented in the "Transaction" type. The reason is that a transaction is a fact and should be immutable. In other words, any data related to transaction should not be changed after the transaction is created. This implementation helps to secure the banking system and ensure the balance of any account is correct by summing up all transaction histories of that account.

---

- Web API:
For displaying transactions, there is a GetAll() method retrieveing all transactions and a GetAll(Customer) method getting transactions of the specified customer. Filtering transactions against transaction time is conducted by applying LINQ search through the above two transaction list. In addition, pagedlist is used to display transactions in a paginated manner.

---

For the DI part, we create a new repository called "s3627805_s3829221_a2_admin". So that we can well manage two projects separately without any interference.
